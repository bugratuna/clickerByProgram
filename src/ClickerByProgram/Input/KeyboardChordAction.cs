using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickerByProgram.Input;

internal sealed class KeyboardChordAction : InputActionBase
{
    public KeyboardChordAction(Keys[] keys, TimeSpan holdDuration, TimeSpan delayBeforeExecution)
        : base(delayBeforeExecution)
    {
        Keys = keys;
        HoldDuration = holdDuration;
    }

    public Keys[] Keys { get; }

    public TimeSpan HoldDuration { get; }

    public override string Description
    {
        get
        {
            var builder = new StringBuilder();
            builder.Append("Keys [");
            builder.Append(string.Join(" + ", Keys.Select(k => k.ToString())));
            builder.Append("] held for ");
            builder.Append(HoldDuration.TotalSeconds.ToString("0.###"));
            builder.Append(" seconds");
            return builder.ToString();
        }
    }

    protected override async Task ExecuteCoreAsync(InputExecutionContext context, CancellationToken cancellationToken)
    {
        if (context.TargetWindow == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.SendKeyboardChord(context.TargetWindow, Keys, true);
        context.Log($"Pressed {string.Join(" + ", Keys.Select(k => k.ToString()))}");

        if (HoldDuration > TimeSpan.Zero)
        {
            await Task.Delay(HoldDuration, cancellationToken).ConfigureAwait(false);
        }

        NativeMethods.SendKeyboardChord(context.TargetWindow, Keys, false);
        context.Log($"Released {string.Join(" + ", Keys.Select(k => k.ToString()))}");
    }
}
