using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate
{
    public static class ConfigurationSeeds
    {
        public static readonly IReadOnlyDictionary<ConfigurationKind, string> ResourceByKind = new Dictionary<ConfigurationKind, string>
        {
            [ConfigurationKind.PARAMETER_CATALOG] = "parameter-catalog-ep-doa-seed.json",
            [ConfigurationKind.DATA_PROFILE] = "data-profile-ep-doa-seed.json",
            [ConfigurationKind.QUALITY_RULES] = "quality-rules-ep-doa-seed.json"
        };

        public static string ContentOf(ConfigurationKind kind)
        {
            var resource = ResourceByKind[kind];

            using var stream = typeof(ConfigurationSeeds).Assembly.GetManifestResourceStream(resource)
                               ?? throw new InvalidOperationException($"Seed resource {resource} is missing from the domain assembly.");
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
