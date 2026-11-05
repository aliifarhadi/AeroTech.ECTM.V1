using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Services
{
    public sealed record ActiveQualityRules(ConfigurationDocument Document, QualityRulesContent Rules);
}
