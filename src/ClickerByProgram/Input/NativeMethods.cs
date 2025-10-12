using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClickerByProgram.Input;

internal static class NativeMethods
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WH_MOUSE_LL = 14;

    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;
    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MBUTTONUP = 0x0208;
    private const int WM_XBUTTONDOWN = 0x020B;
    private const int WM_XBUTTONUP = 0x020C;

    private const int MK_LBUTTON = 0x0001;
    private const int MK_RBUTTON = 0x0002;
    private const int MK_MBUTTON = 0x0010;
    private const int MK_XBUTTON1 = 0x0020;
    private const int MK_XBUTTON2 = 0x0040;

    private const int XBUTTON1 = 0x0001;
    private const int XBUTTON2 = 0x0002;

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KbdLlHookStruct lParam);

    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, ref MsLlHookStruct lParam);

    public static IntPtr SetKeyboardHook(LowLevelKeyboardProc callback)
    {
        using var module = Kernel32.GetModuleHandleSafe();
        return SetWindowsHookEx(WH_KEYBOARD_LL, callback, module.DangerousGetHandle(), 0);
    }

    public static IntPtr SetMouseHook(LowLevelMouseProc callback)
    {
        using var module = Kernel32.GetModuleHandleSafe();
        return SetWindowsHookEx(WH_MOUSE_LL, callback, module.DangerousGetHandle(), 0);
    }

    public static bool RemoveHook(IntPtr hook)
    {
        if (hook == IntPtr.Zero)
        {
            return true;
        }

        return UnhookWindowsHookEx(hook);
    }

    public static IntPtr CallNextKeyboard(IntPtr hook, int nCode, IntPtr wParam, ref KbdLlHookStruct lParam)
    {
        return CallNextHookEx(hook, nCode, wParam, ref lParam);
    }

    public static IntPtr CallNextMouse(IntPtr hook, int nCode, IntPtr wParam, ref MsLlHookStruct lParam)
    {
        return CallNextHookEx(hook, nCode, wParam, ref lParam);
    }

    public static void SendKeyboardChord(IntPtr windowHandle, Keys[] keys, bool keyDown)
    {
        var message = keyDown ? WM_KEYDOWN : WM_KEYUP;
        var sysMessage = keyDown ? WM_SYSKEYDOWN : WM_SYSKEYUP;

        foreach (var key in keys)
        {
            var vk = (int)key;
            var lParam = CreateKeyboardLParam(key, keyDown);
            PostMessage(windowHandle, IsSystemKey(key) ? (uint)sysMessage : (uint)message, new IntPtr(vk), lParam);
        }
    }

    public static void SendMouseMove(IntPtr windowHandle, Point screenPosition)
    {
        var clientPoint = screenPosition;
        ScreenToClient(windowHandle, ref clientPoint);
        var lParam = MakeLParam(clientPoint.X, clientPoint.Y);
        PostMessage(windowHandle, WM_MOUSEMOVE, IntPtr.Zero, lParam);
    }

    public static void SendMouseButton(IntPtr windowHandle, Point screenPosition, MouseButton button, bool buttonDown)
    {
        var clientPoint = screenPosition;
        ScreenToClient(windowHandle, ref clientPoint);
        var lParam = MakeLParam(clientPoint.X, clientPoint.Y);
        var wParam = GetMouseButtonWParam(button);
        var message = GetMouseButtonMessage(button, buttonDown);
        if (button is MouseButton.XButton1 or MouseButton.XButton2)
        {
            var xButtonFlag = button == MouseButton.XButton1 ? XBUTTON1 : XBUTTON2;
            wParam |= (xButtonFlag << 16);
        }

        PostMessage(windowHandle, message, new IntPtr(wParam), lParam);
    }

    private static uint GetMouseButtonMessage(MouseButton button, bool buttonDown)
    {
        return button switch
        {
            MouseButton.Left => buttonDown ? WM_LBUTTONDOWN : WM_LBUTTONUP,
            MouseButton.Right => buttonDown ? WM_RBUTTONDOWN : WM_RBUTTONUP,
            MouseButton.Middle => buttonDown ? WM_MBUTTONDOWN : WM_MBUTTONUP,
            MouseButton.XButton1 or MouseButton.XButton2 => buttonDown ? WM_XBUTTONDOWN : WM_XBUTTONUP,
            _ => WM_LBUTTONDOWN
        };
    }

    private static int GetMouseButtonWParam(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => MK_LBUTTON,
            MouseButton.Right => MK_RBUTTON,
            MouseButton.Middle => MK_MBUTTON,
            MouseButton.XButton1 => MK_XBUTTON1,
            MouseButton.XButton2 => MK_XBUTTON2,
            _ => 0
        };
    }

    private static bool IsSystemKey(Keys key)
    {
        return key is Keys.Menu or Keys.F10;
    }

    private static IntPtr CreateKeyboardLParam(Keys key, bool keyDown)
    {
        var repeatCount = 1;
        var scanCode = MapVirtualKey((uint)key, 0) & 0xFF;
        var extendedFlag = IsExtendedKey(key) ? 1 << 24 : 0;
        var previousState = keyDown ? 0 : 1 << 30;
        var transitionState = keyDown ? 0 : 1 << 31;
        var value = (repeatCount & 0xFFFF) | ((int)scanCode << 16) | extendedFlag | previousState | transitionState;
        return new IntPtr(value);
    }

    private static IntPtr MakeLParam(int low, int high)
    {
        return new IntPtr((high << 16) | (low & 0xFFFF));
    }

    private static bool IsExtendedKey(Keys key)
    {
        return key switch
        {
            Keys.Insert or Keys.Delete or Keys.Home or Keys.End or Keys.PageUp or Keys.PageDown => true,
            Keys.Up or Keys.Down or Keys.Left or Keys.Right => true,
            Keys.NumLock or Keys.Divide or Keys.RControlKey or Keys.RMenu => true,
            Keys.LWin or Keys.RWin or Keys.Apps => true,
            _ => false
        };
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KbdLlHookStruct lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MsLlHookStruct lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll", SetLastError = false)]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [StructLayout(LayoutKind.Sequential)]
    internal struct KbdLlHookStruct
    {
        public int VkCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public IntPtr DwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MsLlHookStruct
    {
        public Point Pt;
        public int MouseData;
        public int Flags;
        public int Time;
        public IntPtr DwExtraInfo;
    }

    private static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        public static SafeModuleHandle GetModuleHandleSafe()
        {
            var handle = GetModuleHandle(null);
            return new SafeModuleHandle(handle, ownsHandle: false);
        }
    }

    internal sealed class SafeModuleHandle : SafeHandle
    {
        public SafeModuleHandle(IntPtr handle, bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(handle);
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            // We never release the handle because it is owned by the OS.
            return true;
        }
    }
}
