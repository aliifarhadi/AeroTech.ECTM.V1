using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public static class EvaluationFingerprint
    {
        public static string Of(BaselineParameter parameter, FlightSource source, ComparisonTolerances tolerances)
        {
            var parts = new[]
            {
                BaselineComparisonEvaluator.CalculationVersion.ToString(CultureInfo.InvariantCulture),
                parameter.Id.ToString(CultureInfo.InvariantCulture),
                source.EngineId.ToString(CultureInfo.InvariantCulture),
                source.FlightId.ToString(CultureInfo.InvariantCulture),
                source.AnalysisRevisionId.ToString(CultureInfo.InvariantCulture),
                source.FlightStabilityPointsId.ToString(CultureInfo.InvariantCulture),
                source.Position.ToString(CultureInfo.InvariantCulture),
                source.FlightDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "-",
                source.SourceFileSha256 ?? "-",
                tolerances.RulesDocumentId?.ToString(CultureInfo.InvariantCulture) ?? "-"
            };

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("|", parts)));

            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
