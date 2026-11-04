using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public static class ConfigurationKinds
    {
        public static ConfigurationKind Parse(string? code)
        {
            foreach (var kind in Enum.GetValues<ConfigurationKind>())
            {
                if (kind.ToString() == code)
                    return kind;
            }

            throw ExceptionFactory.ConfigurationKindNotFound(code);
        }
    }
}
