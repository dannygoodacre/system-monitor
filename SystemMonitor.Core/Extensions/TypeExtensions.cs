using SystemMonitor.Core.CommandQuery;

namespace SystemMonitor.Core.Extensions;

internal static class TypeExtensions
{
    public static bool InheritsFromCommandHandler(this Type type)
    {
        var baseType = type.BaseType;

        while (baseType is not null)
        {
            if (baseType.IsGenericType)
            {
                var definition = baseType.GetGenericTypeDefinition();

                if (definition == typeof(CommandHandler<>) || definition == typeof(CommandHandler<,>))
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool InheritsFromQueryHandler(this Type type)
    {
        var baseType = type.BaseType;

        while (baseType is not null)
        {
            if (baseType.IsGenericType)
            {
                var definition = baseType.GetGenericTypeDefinition();

                if (definition == typeof(QueryHandler<,>))
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}
