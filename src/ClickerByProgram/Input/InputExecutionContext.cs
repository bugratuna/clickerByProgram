using System;

namespace ClickerByProgram.Input;

internal sealed class InputExecutionContext
{
    public InputExecutionContext(IntPtr targetWindow, Action<string>? logger)
    {
        TargetWindow = targetWindow;
        Logger = logger;
    }

    public IntPtr TargetWindow { get; }

    public Action<string>? Logger { get; }

    public void Log(string message)
    {
        Logger?.Invoke(message);
    }
}
