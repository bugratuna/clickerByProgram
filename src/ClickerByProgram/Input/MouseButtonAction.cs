using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickerByProgram.Input;

internal enum MouseButton
{
    Left,
    Right,
    Middle,
    XButton1,
    XButton2
}

internal sealed class MouseButtonAction : InputActionBase
{
    public MouseButtonAction(MouseButton button, Point screenPosition, TimeSpan holdDuration, TimeSpan delayBeforeExecution)
        : base(delayBeforeExecution)
    {
        Button = button;
        ScreenPosition = screenPosition;
        HoldDuration = holdDuration;
    }

    public MouseButton Button { get; }

    public Point ScreenPosition { get; }

    public TimeSpan HoldDuration { get; }

    public override string Description => $"{Button} button at ({ScreenPosition.X}, {ScreenPosition.Y}) for {HoldDuration.TotalSeconds:0.###} seconds";

    protected override async Task ExecuteCoreAsync(InputExecutionContext context, CancellationToken cancellationToken)
    {
        if (context.TargetWindow == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.SendMouseButton(context.TargetWindow, ScreenPosition, Button, true);
        context.Log($"Mouse {Button} down at ({ScreenPosition.X}, {ScreenPosition.Y})");

        if (HoldDuration > TimeSpan.Zero)
        {
            await Task.Delay(HoldDuration, cancellationToken).ConfigureAwait(false);
        }

        NativeMethods.SendMouseButton(context.TargetWindow, ScreenPosition, Button, false);
        context.Log($"Mouse {Button} up at ({ScreenPosition.X}, {ScreenPosition.Y})");
    }
}
