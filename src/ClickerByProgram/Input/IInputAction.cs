using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClickerByProgram.Input;

internal interface IInputAction
{
    string Description { get; }

    TimeSpan DelayBeforeExecution { get; }

    Task ExecuteAsync(InputExecutionContext context, CancellationToken cancellationToken);
}
