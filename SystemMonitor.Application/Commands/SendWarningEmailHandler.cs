using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Abstractions.Services;
using SystemMonitor.Configuration.Options;
using SystemMonitor.Core.CommandQuery;
using SystemMonitor.Core.Common;

namespace SystemMonitor.Application.Commands;

internal sealed class SendWarningEmailHandler(ILogger<SendWarningEmailHandler> logger,
                                              IOptions<EmailOptions> options,
                                              IEmailService emailService,
                                              IUserRepository userRepository,
                                              IEventRepository eventRepository,
                                              IApplicationContext context) : CommandHandler<SendWarningEmailCommand>(logger), ISendWarningEmail
{
    protected override string CommandName => "Send Warning Email";

    protected override async Task<Result> InternalExecuteAsync(SendWarningEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Command '{Command}' started.", CommandName);

        var @event = await eventRepository.GetLatestAsync(command.Resource, cancellationToken);

        if (@event is null)
        {
            return Result.InternalError("No event found.");
        }

        List<string> emails;

        var users = await userRepository.GetAllAsync(cancellationToken);

        if (users.Count != 0)
        {
            emails = users.Select(x => x.Email).ToList();
        }
        else
        {
            logger.LogWarning("Command '{Command}' found no users to contact; using backup email '{BackupEmail}' instead.", CommandName, options.Value.BackupEmail);

            emails = [ options.Value.BackupEmail ];
        }

        var message = $"Issue detected for resource '{@event.Resource}':<br><pre><code>{@event.Message}</code></pre>";

        // foreach (var email in emails)
        // {
        //     await emailService.SendEmailAsync(email, options.Value.Subject, message);
        // }

        // TODO: @event is a model, need to map back to entity or just use the underlying entity.
        @event.UsersNotifiedIds = users.Select(x => x.Id).ToList();

        const int expectedChanges = 0;

        var actualChanges = await context.SaveChangesAsync();

        if (expectedChanges != actualChanges)
        {
            logger.LogWarning("Command '{Command}' wrote an unexpected number of changes to the database: expected '{Expected}', actual '{Actual}'.", CommandName, expectedChanges, actualChanges);
        }

        logger.LogInformation("Command '{Command}' completed, sending an email to: {Recipients}.", CommandName, string.Join(", ", emails));

        return Result.Success();
    }

    public Task<Result> ExecuteAsync(string resource, CancellationToken cancellationToken)
        => base.ExecuteAsync(new SendWarningEmailCommand
        {
            Resource = resource
        }, cancellationToken);
}

public sealed class SendWarningEmailCommand : ICommand
{
    public required string Resource { get; init; }
}

public interface ISendWarningEmail
{
    Task<Result> ExecuteAsync(string resource, CancellationToken cancellationToken = default);
}
