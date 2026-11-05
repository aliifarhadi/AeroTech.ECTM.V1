using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services
{
    public sealed record RecomputeSchedule(RecomputeOutcome Outcome, AnalysisRevision? Revision, long? ProcessingJobId);
}
