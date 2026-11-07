using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain.BaselineAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineAggregate.Entities
{
    public sealed class BaselineParameter : Entity<long>
    {
        private BaselineParameter()
        {
        }

        internal BaselineParameter(long id, long baselineId, BaselineParameterArgs args)
        {
            Id = id;
            BaselineId = baselineId;
            SnapshotKind = args.SnapshotKind;
            Selector = args.Selector;
            Statistic = MeasurementStatistic.Of(args.SnapshotKind);
            ParameterCode = args.ParameterCode;
            Unit = args.Unit;
            RawValue = args.RawValue;
            SuggestedRawValue = args.SuggestedRawValue;
            ValueOrigin = OriginOf(args.RawValue, args.SuggestedRawValue);
            SourceEnginePosition = args.SourceEnginePosition is { } position ? (byte)position : null;
            SourceStartIndex = args.SourceStartIndex;
            SourceEndIndexInclusive = args.SourceEndIndexInclusive;
            SourcePeakIndex = args.SourcePeakIndex;
            ReferenceContext = args.ReferenceContext.ToJson();
            CatalogDocumentId = args.CatalogDocumentId;
            CatalogVersion = args.CatalogVersion;
            SnapshotRulesDocumentId = args.SnapshotRulesDocumentId;
            SnapshotRulesVersion = args.SnapshotRulesVersion;
            Notes = OptionalText.Normalize(args.Notes);
        }

        public long BaselineId { get; private set; }

        public SnapshotKind SnapshotKind { get; private set; }

        public string? Selector { get; private set; }

        public BaselineStatistic Statistic { get; private set; }

        public string ParameterCode { get; private set; } = default!;

        public string? Unit { get; private set; }

        public double? RawValue { get; private set; }

        public double? SuggestedRawValue { get; private set; }

        public BaselineValueOrigin ValueOrigin { get; private set; }

        public byte? SourceEnginePosition { get; private set; }

        public int? SourceStartIndex { get; private set; }

        public int? SourceEndIndexInclusive { get; private set; }

        public int? SourcePeakIndex { get; private set; }

        public string? ReferenceContext { get; private set; }

        public long CatalogDocumentId { get; private set; }

        public int CatalogVersion { get; private set; }

        public long SnapshotRulesDocumentId { get; private set; }

        public int SnapshotRulesVersion { get; private set; }

        public string? Notes { get; private set; }

        public MeasurementContext ReadReferenceContext() => MeasurementContext.FromJson(ReferenceContext);

        public bool HasSameMeasurementAs(BaselineParameter other)
            => SnapshotKind == other.SnapshotKind
               && Selector == other.Selector
               && ParameterCode == other.ParameterCode;

        internal BaselineParameterArgs ToArgs()
            => new(
                SnapshotKind,
                Selector,
                ParameterCode,
                Unit,
                RawValue,
                SuggestedRawValue,
                SourceEnginePosition,
                SourceStartIndex,
                SourceEndIndexInclusive,
                SourcePeakIndex,
                ReadReferenceContext(),
                CatalogDocumentId,
                CatalogVersion,
                SnapshotRulesDocumentId,
                SnapshotRulesVersion,
                Notes);

        private static BaselineValueOrigin OriginOf(double? rawValue, double? suggestedRawValue)
        {
            if (suggestedRawValue is null)
                return BaselineValueOrigin.MANUAL;

            if (rawValue == suggestedRawValue)
                return BaselineValueOrigin.FILE_SUGGESTED;

            return BaselineValueOrigin.FILE_EDITED;
        }
    }
}
