using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ClickerByProgram.Input;

internal sealed class MouseMoveAction : InputActionBase
{
    public MouseMoveAction(Point screenPosition, TimeSpan delayBeforeExecution)
        : base(delayBeforeExecution)
    {
        ScreenPosition = screenPosition;
    }

    public Point ScreenPosition { get; }

    public override string Description => $"Mouse move to ({ScreenPosition.X}, {ScreenPosition.Y})";

    protected override Task ExecuteCoreAsync(InputExecutionContext context, CancellationToken cancellationToken)
    {
        if (context.TargetWindow == IntPtr.Zero)
        {
            return Task.CompletedTask;
        }

        NativeMethods.SendMouseMove(context.TargetWindow, ScreenPosition);
        context.Log($"Moved mouse to ({ScreenPosition.X}, {ScreenPosition.Y})");
        return Task.CompletedTask;
    }
}
