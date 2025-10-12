using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClickerByProgram.Input;

internal abstract class InputActionBase : IInputAction
{
    protected InputActionBase(TimeSpan delayBeforeExecution)
    {
        DelayBeforeExecution = delayBeforeExecution;
    }

    public TimeSpan DelayBeforeExecution { get; }

    public abstract string Description { get; }

    public async Task ExecuteAsync(InputExecutionContext context, CancellationToken cancellationToken)
    {
        if (DelayBeforeExecution > TimeSpan.Zero)
        {
            await Task.Delay(DelayBeforeExecution, cancellationToken).ConfigureAwait(false);
        }

        await ExecuteCoreAsync(context, cancellationToken).ConfigureAwait(false);
    }

    protected abstract Task ExecuteCoreAsync(InputExecutionContext context, CancellationToken cancellationToken);
}
