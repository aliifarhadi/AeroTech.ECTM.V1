using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselinePrimarySelectionAggregate
{
    public sealed class BaselinePrimarySelection : AggregateRoot<long>
    {
        private BaselinePrimarySelection()
        {
        }

        private BaselinePrimarySelection(long id, long engineId, SnapshotKind snapshotKind, string? selector, string parameterCode)
        {
            Id = id;
            EngineId = engineId;
            SnapshotKind = snapshotKind;
            Selector = selector;
            ParameterCode = parameterCode;
        }

        public long EngineId { get; private set; }

        public SnapshotKind SnapshotKind { get; private set; }

        public string? Selector { get; private set; }

        public string ParameterCode { get; private set; } = default!;

        public long BaselineId { get; private set; }

        public long BaselineParameterId { get; private set; }

        public string? SelectedBy { get; private set; }

        public DateTimeOffset SelectedAt { get; private set; }

        public static BaselinePrimarySelection Select(long id, Baseline baseline, BaselineParameter parameter, string? actor, DateTimeOffset now)
        {
            var selection = new BaselinePrimarySelection(id, baseline.EngineId, parameter.SnapshotKind, parameter.Selector, parameter.ParameterCode);
            selection.PointTo(baseline, parameter, actor, now);

            return selection;
        }

        public void PointTo(Baseline baseline, BaselineParameter parameter, string? actor, DateTimeOffset now)
        {
            baseline.EnsurePublished();

            BaselineId = baseline.Id;
            BaselineParameterId = parameter.Id;
            SelectedBy = actor;
            SelectedAt = now;
        }
    }
}
