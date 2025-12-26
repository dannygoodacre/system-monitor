using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Abstractions.Services;
using SystemMonitor.Application.Extensions;
using SystemMonitor.Configuration.Options;
using SystemMonitor.Core.CommandQuery;
using SystemMonitor.Core.Common;

namespace SystemMonitor.Application.Commands;

internal sealed class SendWarningEmailHandler(ILogger<SendWarningEmailHandler> logger,
                                              IOptions<ContactOptions> options,
                                              IEmailService emailService,
                                              ILastStatusRepository repository) : CommandHandler<SendWarningEmailCommand>(logger), ISendWarningEmail
{
    protected override string CommandName => "Send Warning Email";

    protected override async Task<Result> InternalExecuteAsync(SendWarningEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Command '{Command}' started.", CommandName);

        var lastStatus = await repository.GetAsync(command.Resource, cancellationToken);

        if (lastStatus is null)
        {
            return Result.InternalError("No status found.");
        }

        var subject = $"System issue detected: {@lastStatus.Resource}";

        var message = $"Issue detected for resource '{lastStatus.Resource}':<br><pre><code>{lastStatus.Message}</code></pre>";

        await emailService.SendEmailAsync(options.Value.EmailAddress, subject, message);

        logger.LogInformation("Command '{Command}' completed, sending an email to: {Recipients}.", CommandName, options.Value.EmailAddress);

        return Result.Success();
    }

    public Task<Result> ExecuteAsync(string resource, CancellationToken cancellationToken)
        => base.ExecuteAsync(new SendWarningEmailCommand
        {
            Resource = resource
        }, cancellationToken);
}

public interface ISendWarningEmail
{
    Task<Result> ExecuteAsync(string resource, CancellationToken cancellationToken = default);
}

public sealed class SendWarningEmailCommand : ICommand
{
    public required string Resource { get; init; }
}
