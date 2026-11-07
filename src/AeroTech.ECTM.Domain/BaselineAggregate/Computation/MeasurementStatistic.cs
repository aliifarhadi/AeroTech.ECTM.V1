using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineAggregate.Computation
{
    public static class MeasurementStatistic
    {
        public static BaselineStatistic Of(SnapshotKind kind)
            => kind == SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED ? BaselineStatistic.AT_PEAK_SAMPLE : BaselineStatistic.MEAN;
    }
}
