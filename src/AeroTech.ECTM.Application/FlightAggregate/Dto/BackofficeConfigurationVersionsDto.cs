using AeroTech.ECTM.Domain._Shared;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeConfigurationVersionsDto(
        long DataProfileDocumentId,
        int DataProfileVersion,
        long ParameterCatalogDocumentId,
        int ParameterCatalogVersion)
    {
        public static BackofficeConfigurationVersionsDto From(ConfigurationVersions versions)
            => new(versions.DataProfileDocumentId, versions.DataProfileVersion, versions.ParameterCatalogDocumentId, versions.ParameterCatalogVersion);
    }
}
