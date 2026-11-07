using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineAggregate
{
    public sealed class Baseline : AggregateRoot<long>
    {
        private const int FirstRevision = 1;

        private readonly List<BaselineParameter> _parameters = new();

        private Baseline()
        {
        }

        private Baseline(long id, long engineId, long lineageId, int revisionNumber, long? previousBaselineId, string? actor, DateTimeOffset now)
        {
            Id = id;
            EngineId = engineId;
            LineageId = lineageId;
            RevisionNumber = revisionNumber;
            PreviousBaselineId = previousBaselineId;
            State = BaselineState.DRAFT;
            DraftRevision = 1;
            CreatedBy = actor;
            CreatedAt = now;
        }

        public long EngineId { get; private set; }

        public long LineageId { get; private set; }

        public int RevisionNumber { get; private set; }

        public long? PreviousBaselineId { get; private set; }

        public BaselineState State { get; private set; }

        public int DraftRevision { get; private set; }

        public string Name { get; private set; } = default!;

        public DateOnly? DeclaredEffectiveDate { get; private set; }

        public long? ShopVisitEventId { get; private set; }

        public bool FirstPostShopDeclared { get; private set; }

        public string? SourceFileName { get; private set; }

        public string? SourceFileSha256 { get; private set; }

        public string? Notes { get; private set; }

        public string? CreatedBy { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public string? PublishedBy { get; private set; }

        public DateTimeOffset? PublishedAt { get; private set; }

        public string? RetiredBy { get; private set; }

        public DateTimeOffset? RetiredAt { get; private set; }

        public string? RetirementReason { get; private set; }

        public IReadOnlyCollection<BaselineParameter> Parameters => _parameters.AsReadOnly();

        public bool IsEvaluated => State != BaselineState.DRAFT;

        public BaselineInputProvenance InputProvenance => BaselineProvenance.Of(_parameters.Select(parameter => parameter.ValueOrigin));

        public static Baseline Draft(long id, long engineId, BaselineArgs args, IReadOnlyList<BaselineParameterArgs> parameters, IIdGenerator ids, string? actor, DateTimeOffset now)
        {
            var baseline = new Baseline(id, engineId, id, FirstRevision, null, actor, now);
            baseline.Describe(args);
            baseline.ReplaceParameters(parameters, ids);

            return baseline;
        }

        public void Revise(int expectedDraftRevision, BaselineArgs args, IReadOnlyList<BaselineParameterArgs> parameters, IIdGenerator ids)
        {
            EnsureCurrentDraft(expectedDraftRevision);

            Describe(args);
            ReplaceParameters(parameters, ids);
            DraftRevision++;
        }

        public IReadOnlyList<BaselineIssue> PublicationIssues()
        {
            var issues = new List<BaselineIssue>();

            if (_parameters.Count == 0)
                issues.Add(BaselineIssue.NO_PARAMETER);

            if (_parameters.Any(parameter => parameter.RawValue is null))
                issues.Add(BaselineIssue.VALUE_MISSING);

            return issues;
        }

        public void Publish(int expectedDraftRevision, string? actor, DateTimeOffset now)
        {
            EnsureCurrentDraft(expectedDraftRevision);

            var issues = PublicationIssues();

            if (issues.Count > 0)
                throw ExceptionFactory.BaselineCannotBePublished(Id, string.Join(", ", issues));

            State = BaselineState.PUBLISHED;
            PublishedBy = actor;
            PublishedAt = now;
        }

        public void Retire(string reason, string? actor, DateTimeOffset now)
        {
            if (State != BaselineState.PUBLISHED)
                throw ExceptionFactory.BaselineIsNotPublished(Id, State);

            State = BaselineState.RETIRED;
            RetiredBy = actor;
            RetiredAt = now;
            RetirementReason = reason.Trim();
        }

        public Baseline CloneAsDraft(long id, int revisionNumber, IIdGenerator ids, string? actor, DateTimeOffset now)
        {
            var clone = new Baseline(id, EngineId, LineageId, revisionNumber, Id, actor, now);
            clone.Describe(DescriptionArgs());
            clone.ReplaceParameters(_parameters.Select(parameter => parameter.ToArgs()).ToList(), ids);

            return clone;
        }

        public BaselineParameter ParameterOf(long baselineParameterId)
            => _parameters.FirstOrDefault(parameter => parameter.Id == baselineParameterId)
               ?? throw ExceptionFactory.BaselineParameterNotFound(baselineParameterId);

        public void EnsurePublished()
        {
            if (State != BaselineState.PUBLISHED)
                throw ExceptionFactory.BaselineIsNotPublished(Id, State);
        }

        private void EnsureCurrentDraft(int expectedDraftRevision)
        {
            if (State != BaselineState.DRAFT)
                throw ExceptionFactory.BaselineIsNotDraft(Id, State);

            if (expectedDraftRevision != DraftRevision)
                throw ExceptionFactory.BaselineDraftRevisionStale(Id, expectedDraftRevision, DraftRevision);
        }

        private void Describe(BaselineArgs args)
        {
            Name = args.Name.Trim();
            DeclaredEffectiveDate = args.DeclaredEffectiveDate;
            ShopVisitEventId = args.ShopVisitEventId;
            FirstPostShopDeclared = args.FirstPostShopDeclared;
            SourceFileName = OptionalText.Normalize(args.SourceFileName);
            SourceFileSha256 = OptionalText.Normalize(args.SourceFileSha256)?.ToLowerInvariant();
            Notes = OptionalText.Normalize(args.Notes);
        }

        private BaselineArgs DescriptionArgs()
            => new(Name, DeclaredEffectiveDate, ShopVisitEventId, FirstPostShopDeclared, SourceFileName, SourceFileSha256, Notes);

        private void ReplaceParameters(IReadOnlyList<BaselineParameterArgs> parameters, IIdGenerator ids)
        {
            BaselineParameterCheck.Ensure(parameters);

            _parameters.Clear();

            foreach (var parameter in parameters)
                _parameters.Add(new BaselineParameter(ids.NewId(), Id, parameter));
        }
    }
}
