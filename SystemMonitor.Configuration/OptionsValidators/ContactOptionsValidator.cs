using Microsoft.Extensions.Options;
using SystemMonitor.Configuration.Options;

namespace SystemMonitor.Configuration.OptionsValidators;

public class ContactOptionsValidator : IValidateOptions<ContactOptions>
{
    public ValidateOptionsResult Validate(string? name, ContactOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.EmailAddress))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.EmailAddress)}' is required.");
        }

        if (string.IsNullOrWhiteSpace(options.PhoneNumber))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.PhoneNumber)}' is required.");
        }

        return ValidateOptionsResult.Success;
    }
}
