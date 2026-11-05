namespace AeroTech.ECTM.Domain._Shared
{
    public sealed record ConfigurationVersions(
        long DataProfileDocumentId,
        int DataProfileVersion,
        long ParameterCatalogDocumentId,
        int ParameterCatalogVersion);
}
