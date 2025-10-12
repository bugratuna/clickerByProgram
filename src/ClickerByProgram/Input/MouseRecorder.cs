using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ClickerByProgram.Input;

internal sealed class MouseRecorder : IDisposable
{
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;
    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MBUTTONUP = 0x0208;
    private const int WM_XBUTTONDOWN = 0x020B;
    private const int WM_XBUTTONUP = 0x020C;

    private readonly Stopwatch _stopwatch = new();
    private readonly Dictionary<MouseButton, MouseButtonState> _buttonStates = new();
    private TimeSpan _lastEventTime;
    private Point _lastMovePoint;
    private TimeSpan _lastMoveTime;

    private NativeMethods.LowLevelMouseProc? _callback;
    private IntPtr _hookHandle;

    public event EventHandler<InputActionRecordedEventArgs>? ActionRecorded;

    public bool IsRecording => _hookHandle != IntPtr.Zero;

    public void Start()
    {
        if (IsRecording)
        {
            return;
        }

        _callback = HandleMouseEvent;
        _hookHandle = NativeMethods.SetMouseHook(_callback);
        if (_hookHandle == IntPtr.Zero)
        {
            _callback = null;
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error, "Unable to install mouse hook.");
        }
        _stopwatch.Restart();
        _lastEventTime = TimeSpan.Zero;
        _lastMovePoint = Point.Empty;
        _lastMoveTime = TimeSpan.Zero;
        _buttonStates.Clear();
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
        _buttonStates.Clear();
    }

    private IntPtr HandleMouseEvent(int nCode, IntPtr wParam, ref NativeMethods.MsLlHookStruct lParam)
    {
        if (nCode >= 0)
        {
            var message = wParam.ToInt32();
            var point = lParam.Pt;
            var now = _stopwatch.Elapsed;

            switch (message)
            {
                case WM_MOUSEMOVE:
                    HandleMouseMove(point, now);
                    break;
                case WM_LBUTTONDOWN:
                    HandleButtonDown(MouseButton.Left, point, now);
                    break;
                case WM_LBUTTONUP:
                    HandleButtonUp(MouseButton.Left, point, now);
                    break;
                case WM_RBUTTONDOWN:
                    HandleButtonDown(MouseButton.Right, point, now);
                    break;
                case WM_RBUTTONUP:
                    HandleButtonUp(MouseButton.Right, point, now);
                    break;
                case WM_MBUTTONDOWN:
                    HandleButtonDown(MouseButton.Middle, point, now);
                    break;
                case WM_MBUTTONUP:
                    HandleButtonUp(MouseButton.Middle, point, now);
                    break;
                case WM_XBUTTONDOWN:
                    var xDownButton = (lParam.MouseData >> 16) switch
                    {
                        0x0001 => MouseButton.XButton1,
                        0x0002 => MouseButton.XButton2,
                        _ => MouseButton.XButton1
                    };
                    HandleButtonDown(xDownButton, point, now);
                    break;
                case WM_XBUTTONUP:
                    var xUpButton = (lParam.MouseData >> 16) switch
                    {
                        0x0001 => MouseButton.XButton1,
                        0x0002 => MouseButton.XButton2,
                        _ => MouseButton.XButton1
                    };
                    HandleButtonUp(xUpButton, point, now);
                    break;
            }
        }

        return NativeMethods.CallNextMouse(_hookHandle, nCode, wParam, ref lParam);
    }

    private void HandleMouseMove(Point point, TimeSpan now)
    {
        UpdateButtonPositions(point);

        var distance = Math.Abs(point.X - _lastMovePoint.X) + Math.Abs(point.Y - _lastMovePoint.Y);
        var timeSinceLast = now - _lastMoveTime;

        if (distance < 2 && timeSinceLast < TimeSpan.FromMilliseconds(15))
        {
            return;
        }

        var delay = now - _lastEventTime;
        _lastEventTime = now;
        _lastMovePoint = point;
        _lastMoveTime = now;

        var action = new MouseMoveAction(point, delay);
        ActionRecorded?.Invoke(this, new InputActionRecordedEventArgs(action));
    }

    private void HandleButtonDown(MouseButton button, Point point, TimeSpan now)
    {
        var delay = now - _lastEventTime;
        _lastEventTime = now;
        _buttonStates[button] = new MouseButtonState(now, point, delay);
    }

    private void HandleButtonUp(MouseButton button, Point point, TimeSpan now)
    {
        if (_buttonStates.TryGetValue(button, out var state))
        {
            var holdDuration = now - state.StartTime;
            _lastEventTime = now;
            _buttonStates.Remove(button);

            var action = new MouseButtonAction(button, state.Position, holdDuration, state.DelayBeforeStart);
            ActionRecorded?.Invoke(this, new InputActionRecordedEventArgs(action));
        }
    }

    private void UpdateButtonPositions(Point point)
    {
        var buttons = new List<MouseButton>(_buttonStates.Keys);
        foreach (var button in buttons)
        {
            var state = _buttonStates[button];
            _buttonStates[button] = state with { Position = point };
        }
    }

    public void Dispose()
    {
        Stop();
    }

    private record struct MouseButtonState(TimeSpan StartTime, Point Position, TimeSpan DelayBeforeStart);
}
