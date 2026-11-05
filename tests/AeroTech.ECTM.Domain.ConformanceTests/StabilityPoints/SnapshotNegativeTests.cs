using System.Text.Json;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.StabilityPoints;

public sealed class SnapshotNegativeTests
{
    private const string Flight6951 = "2026-09-30_EP-DOA_6951.csv";

    private static readonly byte[] Original = Recordings.RealBytes(Flight6951);
    private static readonly StabilityPointsResult OriginalResult = SnapshotParity.Evaluate(Flight6951, Original).Points;

    [Fact]
    public void SN_NEG_01_no_egt_in_the_engine_1_takeoff_window_is_not_evaluable_and_leaves_engine_2_alone()
    {
        var takeoff = OriginalResult.TakeoffN1Peaks.Single(peak => peak.Position == 1);
        var egt = Recordings.OrdinalOf("EGT_C[1]");
        var withoutEgt = Recordings.WithLines(Original, lines =>
        {
            for (var row = takeoff.StartIndex!.Value; row <= takeoff.EndIndexInclusive!.Value; row++)
                lines[row + 1] = Recordings.WithCell(lines[row + 1], egt, "");

            return lines;
        });

        var result = SnapshotParity.Evaluate(Flight6951, withoutEgt).Points;
        var engineOne = result.TakeoffN1Peaks.Single(peak => peak.Position == 1);

        Assert.Equal((SnapshotAvailability.NOT_EVALUABLE, SnapshotReason.NO_EGT_IN_SELECTED_WINDOW), (engineOne.Availability, engineOne.Reason!.Value));
        Assert.Equal(Json(OriginalResult.TakeoffN1Peaks.Single(peak => peak.Position == 2)), Json(result.TakeoffN1Peaks.Single(peak => peak.Position == 2)));
    }

    [Fact]
    public void SN_NEG_02_holes_in_engine_2_N1_leave_engine_1_identical()
    {
        var n1 = Recordings.OrdinalOf("N1_PCT[2]");
        var withHoles = Recordings.WithLines(Original, lines =>
        {
            for (var line = 1; line < lines.Count; line += 7)
            {
                if (lines[line].Length > 0)
                    lines[line] = Recordings.WithCell(lines[line], n1, "");
            }

            return lines;
        });

        var result = SnapshotParity.Evaluate(Flight6951, withHoles).Points;

        Assert.Equal(Json(EngineOne(OriginalResult)), Json(EngineOne(result)));
        Assert.NotEqual(Json(EngineTwo(OriginalResult)), Json(EngineTwo(result)));
    }

    [Fact]
    public void SN_05_flight_24_pairs_the_central_60_s_of_the_joint_block()
    {
        var result = SnapshotParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24)).Points;
        var paired = result.Cruise.Single(entry => entry.Selector == "OPERATOR_300S").Paired;

        Assert.Equal((1718, 2263), (paired.JointStartIndex, paired.JointEndIndexInclusive));
        Assert.Equal((1961, 2020, 60), (paired.ReportStartIndex, paired.ReportEndIndexInclusive, paired.MatchedSamples));
        Assert.Equal(("EGT_C[2]", "EGT_C[1]"), (paired.MinuendKey, paired.SubtrahendKey));
    }

    [Fact]
    public void A_cruise_window_carries_its_gate_values_and_the_nearest_rejected_neighbours()
    {
        var engine = OriginalResult.Cruise.Single(entry => entry.Selector == "OPERATOR_300S").Engines.Single(snapshot => snapshot.Position == 1);
        var gates = engine.Gates.ReportWindowGates.ToDictionary(gate => gate.Gate);

        Assert.Equal(SnapshotAvailability.AVAILABLE, engine.Availability);
        Assert.All(engine.Gates.ReportWindowGates, gate => Assert.True(gate.Passes, $"{gate.Gate} {gate.Actual} > {gate.Limit}"));
        Assert.Equal((1.0, 200, 0.02, 1), (gates[CruiseGate.N1_RANGE].Limit, gates[CruiseGate.ALTITUDE_RANGE].Limit, gates[CruiseGate.MACH_RANGE].Limit, gates[CruiseGate.CONFIGURATION_CHANGED].Limit));
        Assert.NotEmpty(engine.Gates.NearestRejected);
        Assert.True(engine.Gates.RejectedWindowEnds[CruiseGate.NOT_CRUISE] > 0);
    }

    [Fact]
    public void Without_a_phase_result_nothing_is_evaluated()
    {
        var (input, quality) = Phases.PhaseParity.QualityOf(Flight6951, Original);

        var result = StabilityPointsEvaluator.Evaluate(SnapshotParity.SeedRules, input, quality, null).Points;

        Assert.Equal(SnapshotReason.NO_PHASE_RESULT, result.NotEvaluableReason);
        Assert.Empty(result.Cruise);
    }

    private static object EngineOne(StabilityPointsResult result) => EngineOf(result, 1);

    private static object EngineTwo(StabilityPointsResult result) => EngineOf(result, 2);

    private static object EngineOf(StabilityPointsResult result, int position)
        => new
        {
            Takeoff = result.TakeoffN1Peaks.Single(peak => peak.Position == position),
            Egt = result.TakeoffEgtPeaks.Single(peak => peak.Position == position),
            Cruise = result.Cruise.Select(selector => selector.Engines.Single(engine => engine.Position == position)).ToList()
        };

    private static string Json(object value) => JsonSerializer.Serialize(value);
}
