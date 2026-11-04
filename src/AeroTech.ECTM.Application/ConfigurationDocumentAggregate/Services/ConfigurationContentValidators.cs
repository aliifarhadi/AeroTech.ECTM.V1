using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class ConfigurationContentValidators
    {
        private readonly IReadOnlyDictionary<ConfigurationKind, IConfigurationContentValidator> _byKind;

        public ConfigurationContentValidators(IEnumerable<IConfigurationContentValidator> validators)
            => _byKind = validators.ToDictionary(validator => validator.Kind);

        public bool IsAuthorable(ConfigurationKind kind) => _byKind.ContainsKey(kind);

        public IConfigurationContentValidator For(ConfigurationKind kind)
        {
            if (_byKind.TryGetValue(kind, out var validator))
                return validator;

            throw ExceptionFactory.ConfigurationKindNotAuthorable(kind, ConfigurationKinds.DeliveryPhase[kind]);
        }
    }
}
