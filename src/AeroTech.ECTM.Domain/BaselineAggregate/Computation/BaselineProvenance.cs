using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineAggregate.Computation
{
    public static class BaselineProvenance
    {
        public static BaselineInputProvenance Of(IEnumerable<BaselineValueOrigin> origins)
        {
            var distinct = origins.Distinct().ToList();

            var hasManualValue = distinct.Contains(BaselineValueOrigin.MANUAL);
            var hasFileValue = distinct.Any(origin => origin != BaselineValueOrigin.MANUAL);

            if (hasManualValue && hasFileValue)
                return BaselineInputProvenance.MIXED;

            if (hasFileValue)
                return BaselineInputProvenance.FILE_PREFILLED;

            return BaselineInputProvenance.MANUAL;
        }
    }
}
