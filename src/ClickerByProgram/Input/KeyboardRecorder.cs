using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClickerByProgram.Input;

internal sealed class KeyboardRecorder : IDisposable
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    private readonly Stopwatch _stopwatch = new();
    private readonly HashSet<Keys> _activeKeys = new();
    private readonly HashSet<Keys> _chordKeys = new();
    private TimeSpan _chordStartTime;
    private TimeSpan _lastEventTime;

    private NativeMethods.LowLevelKeyboardProc? _callback;
    private IntPtr _hookHandle;

    public event EventHandler<InputActionRecordedEventArgs>? ActionRecorded;

    public bool IsRecording => _hookHandle != IntPtr.Zero;

    public void Start()
    {
        if (IsRecording)
        {
            return;
        }

        _callback = HandleKeyboardEvent;
        _hookHandle = NativeMethods.SetKeyboardHook(_callback);
        if (_hookHandle == IntPtr.Zero)
        {
            _callback = null;
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error, "Unable to install keyboard hook.");
        }
        _stopwatch.Restart();
        _lastEventTime = TimeSpan.Zero;
        _activeKeys.Clear();
        _chordKeys.Clear();
        _chordStartTime = TimeSpan.Zero;
    }

    public void Stop()
    {
        if (!IsRecording)
        {
            return;
        }

        NativeMethods.RemoveHook(_hookHandle);
        _hookHandle = IntPtr.Zero;
        _callback = null;
        _activeKeys.Clear();
        _chordKeys.Clear();
    }

    private IntPtr HandleKeyboardEvent(int nCode, IntPtr wParam, ref NativeMethods.KbdLlHookStruct lParam)
    {
        if (nCode >= 0)
        {
            var key = (Keys)lParam.VkCode;
            var message = wParam.ToInt32();
            var isKeyDown = message is WM_KEYDOWN or WM_SYSKEYDOWN;
            var isKeyUp = message is WM_KEYUP or WM_SYSKEYUP;

            if (isKeyDown)
            {
                if (_activeKeys.Count == 0)
                {
                    _chordKeys.Clear();
                    _chordStartTime = _stopwatch.Elapsed;
                }

                _activeKeys.Add(key);
                _chordKeys.Add(key);
            }
            else if (isKeyUp)
            {
                if (_activeKeys.Contains(key))
                {
                    _activeKeys.Remove(key);

                    if (_activeKeys.Count == 0)
                    {
                        var elapsed = _stopwatch.Elapsed;
                        var holdDuration = elapsed - _chordStartTime;
                        var delay = _chordStartTime - _lastEventTime;
                        _lastEventTime = elapsed;
                        var recordedKeys = _chordKeys
                            .OrderBy(GetKeyPriority)
                            .ThenBy(k => k.ToString(), StringComparer.Ordinal)
                            .ToArray();
                        _chordKeys.Clear();

                        if (recordedKeys.Length > 0)
                        {
                            var action = new KeyboardChordAction(recordedKeys, holdDuration, delay);
                            ActionRecorded?.Invoke(this, new InputActionRecordedEventArgs(action));
                        }
                    }
                }
            }
        }

        return NativeMethods.CallNextKeyboard(_hookHandle, nCode, wParam, ref lParam);
    }

    public void Dispose()
    {
        Stop();
    }

    private static int GetKeyPriority(Keys key)
    {
        return key switch
        {
            Keys.LControlKey or Keys.RControlKey or Keys.ControlKey => 0,
            Keys.LShiftKey or Keys.RShiftKey or Keys.ShiftKey => 0,
            Keys.LMenu or Keys.RMenu or Keys.Menu => 0,
            Keys.LWin or Keys.RWin => 0,
            _ => 1
        };
    }
}
