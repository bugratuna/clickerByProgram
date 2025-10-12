using System;

namespace ClickerByProgram.Input;

internal sealed class InputActionRecordedEventArgs : EventArgs
{
    public InputActionRecordedEventArgs(IInputAction action)
    {
        Action = action;
    }

    public IInputAction Action { get; }
}
