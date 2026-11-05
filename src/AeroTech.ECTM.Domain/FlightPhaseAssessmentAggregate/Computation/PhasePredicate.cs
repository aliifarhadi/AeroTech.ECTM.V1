using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "operator")]
    [JsonDerivedType(typeof(ComparePredicate), ComparePredicate.Operator)]
    [JsonDerivedType(typeof(AbsComparePredicate), AbsComparePredicate.Operator)]
    [JsonDerivedType(typeof(EqualsPredicate), EqualsPredicate.Operator)]
    [JsonDerivedType(typeof(AllPredicate), AllPredicate.Operator)]
    [JsonDerivedType(typeof(AnyPredicate), AnyPredicate.Operator)]
    [JsonDerivedType(typeof(NotPredicate), NotPredicate.Operator)]
    [JsonDerivedType(typeof(InConditionPredicate), InConditionPredicate.Operator)]
    [JsonDerivedType(typeof(InMaskPredicate), InMaskPredicate.Operator)]
    public abstract record PhasePredicate
    {
        public abstract bool Holds(PhaseFrame frame, int index);

        public abstract PredicateTrace Trace(PhaseFrame frame, int index);

        public abstract void Check(RuleDocumentCheck check, string path);

        public static void CheckPresent(PhasePredicate? predicate, RuleDocumentCheck check, string path)
        {
            if (predicate is null)
                check.Required(null, path);
            else
                predicate.Check(check, path);
        }

        public static void CheckEach(IReadOnlyList<PhasePredicate>? predicates, RuleDocumentCheck check, string path)
        {
            check.Entries(predicates, path, allowEmpty: false);

            for (var item = 0; item < (predicates?.Count ?? 0); item++)
                CheckPresent(predicates![item], check, $"{path}[{item}]");
        }
    }
}
