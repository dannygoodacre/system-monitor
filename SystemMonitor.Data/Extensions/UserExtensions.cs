using Microsoft.AspNetCore.Identity;
using SystemMonitor.Domain.Models;

namespace SystemMonitor.Data.Extensions;

internal static class UserExtensions
{
    public static User ToModel(this IdentityUser value)
        => new()
        {
            Id = value.Id,
            Email = value.Email!,
        };
}
