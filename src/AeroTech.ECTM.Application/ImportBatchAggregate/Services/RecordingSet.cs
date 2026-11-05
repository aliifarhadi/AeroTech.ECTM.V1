using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed record RecordingSet(ConfigurationVersions Versions, DataProfileContent Profile, ValidatedCatalog Catalog)
    {
        public char Delimiter => Profile.Delimiter![0];
    }
}
