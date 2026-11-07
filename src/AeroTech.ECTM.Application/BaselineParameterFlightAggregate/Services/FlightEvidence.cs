using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed record FlightEvidence(
        Flight Flight,
        FlightStabilityPoints Points,
        StabilityPointsResult Result,
        FlightSource Source,
        ValidatedCatalog? Catalog)
    {
        public bool IsFinal => Flight.State == FlightState.ACCEPTED && Points.Status == ResultStatus.FINAL;

        public ObservedMeasurement Measure(BaselineParameter parameter)
        {
            var unit = Catalog?.Find(parameter.ParameterCode)?.Unit;

            return ObservedMeasurement.Read(Source, Result, parameter, unit);
        }
    }
}
