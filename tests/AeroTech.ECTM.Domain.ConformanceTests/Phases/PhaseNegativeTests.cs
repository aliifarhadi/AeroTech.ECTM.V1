using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Phases;

public sealed class PhaseNegativeTests
{
    private const string Flight6951 = "2026-09-30_EP-DOA_6951.csv";

    private static readonly byte[] Original = Recordings.RealBytes(Flight6951);
    private static readonly PhaseResult OriginalResult = PhaseParity.Evaluate(Flight6951, Original);

    [Fact]
    public void PH04a_a_one_row_air_bounce_after_touchdown_moves_touchdown_to_the_change_that_holds()
    {
        var touchdown = OriginalResult.AnchorOf(PhaseParity.Touchdown)!.Index!.Value;
        var airGround = Recordings.OrdinalOf("AIR_GROUND");
        var bounced = Recordings.WithLines(Original, lines =>
        {
            lines[RowLine(touchdown + 2)] = Recordings.WithCell(lines[RowLine(touchdown + 2)], airGround, "AIR");
            return lines;
        });

        var result = PhaseParity.Evaluate(Flight6951, bounced);
        var airLabels = new[] { FlightPhaseCode.CLIMB, FlightPhaseCode.DESCENT, FlightPhaseCode.LEVEL, FlightPhaseCode.CRUISE };

        Assert.Equal(touchdown + 3, result.AnchorOf(PhaseParity.Touchdown)!.Index);
        Assert.Equal(AnchorBasis.AIRGROUND_DISCRETE_LAGGED, result.AnchorOf(PhaseParity.Touchdown)!.Basis);
        Assert.Single(result.Segments, segment => segment.PhaseCode == FlightPhaseCode.LANDING_ROLL);
        Assert.DoesNotContain(result.Segments, segment => segment.StartIndex >= touchdown && airLabels.Contains(segment.PhaseCode));
        AssertFullPartition(result);
    }

    [Fact]
    public void PH04b_a_three_second_gap_in_cruise_makes_both_neighbours_unknown_and_splits_the_segments()
    {
        var cruise = OriginalResult.Segments.Where(segment => segment.PhaseCode == FlightPhaseCode.CRUISE).MaxBy(segment => segment.Seconds)!;
        var middle = (cruise.StartIndex + cruise.EndIndexInclusive) / 2;
        var withGap = Recordings.WithLines(Original, lines =>
        {
            lines.RemoveRange(RowLine(middle + 1), 3);
            return lines;
        });

        var result = PhaseParity.Evaluate(Flight6951, withGap);
        var gapStart = middle + 1;
        var before = SegmentAt(result, gapStart - 1);
        var after = SegmentAt(result, gapStart);

        Assert.Equal((FlightPhaseCode.UNKNOWN, FlightPhaseCode.UNKNOWN), (before.PhaseCode, after.PhaseCode));
        Assert.Contains(PhaseUnknownReason.RECORDING_GAP, before.UnknownReasons);
        Assert.Contains(PhaseUnknownReason.RECORDING_GAP, after.UnknownReasons);
        Assert.Equal(gapStart, after.StartIndex);
        Assert.DoesNotContain(result.Segments, segment => segment.StartIndex < gapStart && segment.EndIndexInclusive >= gapStart);
        Assert.All(Enumerable.Range(gapStart - 13, 26), index => Assert.Equal(FlightPhaseCode.UNKNOWN, SegmentAt(result, index).PhaseCode));
        Assert.Equal(FlightPhaseCode.CRUISE, SegmentAt(result, gapStart - 14).PhaseCode);
        Assert.Equal(FlightPhaseCode.CRUISE, SegmentAt(result, gapStart + 13).PhaseCode);
        AssertFullPartition(result);
    }

    [Fact]
    public void PH04c_a_missing_radio_height_gives_no_lift_off_and_still_a_full_partition()
    {
        var radioHeight = Recordings.OrdinalOf("RADIO_HEIGHT_FT");
        var withoutRadioHeight = Recordings.WithLines(Original, lines => lines
            .Select((line, position) => position == 0 || line.Length == 0 ? line : Recordings.WithCell(line, radioHeight, ""))
            .ToList());

        var result = PhaseParity.Evaluate(Flight6951, withoutRadioHeight);
        var liftoff = result.AnchorOf(PhaseParity.Liftoff)!;

        Assert.Null(liftoff.Index);
        Assert.Equal(AnchorBasis.NOT_FOUND, liftoff.Basis);
        Assert.DoesNotContain(result.Segments, segment => segment.PhaseCode is FlightPhaseCode.TAKEOFF or FlightPhaseCode.TAKEOFF_ROLL or FlightPhaseCode.TAXI_OUT);
        Assert.Contains(result.Segments, segment => segment.PhaseCode == FlightPhaseCode.CRUISE);
        Assert.NotNull(result.AnchorOf(PhaseParity.Touchdown)!.Index);
        AssertFullPartition(result);
    }

    [Fact]
    public void PH04d_a_frozen_altitude_never_produces_a_climb()
    {
        var climb = OriginalResult.Segments.Where(segment => segment.PhaseCode == FlightPhaseCode.CLIMB).MaxBy(segment => segment.Seconds)!;
        var altitude = Recordings.OrdinalOf("PRESSURE_ALT_FT");
        var frozen = Recordings.WithLines(Original, lines =>
        {
            var held = lines[RowLine(climb.StartIndex)].Split(',')[altitude];

            for (var row = climb.StartIndex; row <= climb.EndIndexInclusive; row++)
                lines[RowLine(row)] = Recordings.WithCell(lines[RowLine(row)], altitude, held);

            return lines;
        });

        var result = PhaseParity.Evaluate(Flight6951, frozen);

        Assert.DoesNotContain(result.Segments, segment => segment.PhaseCode == FlightPhaseCode.CLIMB
            && segment.EndIndexInclusive >= climb.StartIndex + 15
            && segment.StartIndex <= climb.EndIndexInclusive - 15);
        AssertFullPartition(result);
    }

    [Fact]
    public void PH04e_a_short_climb_between_descents_without_toga_becomes_unknown()
    {
        var labels = ClimbBetweenDescents(climbSeconds: 40);
        var frame = SyntheticFrame(100, toga: index => "Not Engaged");

        GoAroundGuard().Apply(frame, labels);

        Assert.All(Enumerable.Range(30, 40), index => Assert.Equal(FlightPhaseCode.UNKNOWN, labels.Labels[index]));
        Assert.All(Enumerable.Range(30, 40), index => Assert.Equal(PhaseUnknownReason.UNCORROBORATED_CLIMB_IN_DESCENT, labels.UnknownReasons[index]));
        Assert.Equal(UncorroboratedClimbInDescentUnknown.Operator, labels.ChangedBy[30]);
    }

    [Fact]
    public void PH04f_a_short_climb_between_descents_with_toga_is_kept_as_a_go_around()
    {
        var labels = ClimbBetweenDescents(climbSeconds: 40);
        var frame = SyntheticFrame(100, toga: index => index == 45 ? "Engaged" : "Not Engaged");

        GoAroundGuard().Apply(frame, labels);

        Assert.All(Enumerable.Range(30, 40), index => Assert.Equal(FlightPhaseCode.CLIMB, labels.Labels[index]));
    }

    [Fact]
    public void A_climb_longer_than_the_guard_limit_is_kept_without_toga()
    {
        var labels = ClimbBetweenDescents(climbSeconds: 61);
        var frame = SyntheticFrame(121, toga: index => "Not Engaged");

        GoAroundGuard().Apply(frame, labels);

        Assert.All(Enumerable.Range(30, 61), index => Assert.Equal(FlightPhaseCode.CLIMB, labels.Labels[index]));
    }

    [Theory]
    [InlineData(TransitionPick.FIRST, 20)]
    [InlineData(TransitionPick.LAST, 50)]
    public void The_touchdown_pick_chooses_between_the_first_and_the_last_change_that_holds(TransitionPick pick, int expected)
    {
        var touchdown = (SustainedTransition)PhaseParity.SeedRules.Anchors!.Single(anchor => anchor.Code == PhaseParity.Touchdown) with { Pick = pick };
        var frame = SyntheticFrame(60, airGround: index => index is < 20 or (>= 30 and < 50) ? "AIR" : "GND");

        frame.AddAnchor(new AnchorResult(PhaseParity.Liftoff, FirstUpwardCrossing.Operator, null, null, AnchorBasis.NOT_FOUND, "RADIO_HEIGHT_FT"));

        Assert.Equal(expected, touchdown.Find(frame).Index);
    }

    private static UncorroboratedClimbInDescentUnknown GoAroundGuard()
        => PhaseParity.SeedRules.PostProcessing!.OfType<UncorroboratedClimbInDescentUnknown>().Single();

    private static PhaseLabels ClimbBetweenDescents(int climbSeconds)
    {
        var count = 30 + climbSeconds + 30;
        var labels = new PhaseLabels(count);

        for (var index = 0; index < count; index++)
        {
            var isClimb = index >= 30 && index < 30 + climbSeconds;
            labels.Assign(index, isClimb ? FlightPhaseCode.CLIMB : FlightPhaseCode.DESCENT, null, isClimb ? "R-CLIMB" : "R-DESCENT");
        }

        return labels;
    }

    private static PhaseFrame SyntheticFrame(int count, Func<int, string>? toga = null, Func<int, string>? airGround = null)
    {
        var texts = new Dictionary<string, IReadOnlyList<string?>>();

        if (toga is not null)
            texts["TOGA"] = Enumerable.Range(0, count).Select(toga).ToList();

        if (airGround is not null)
            texts["AIR_GROUND"] = Enumerable.Range(0, count).Select(airGround).ToList();

        var elapsed = Enumerable.Range(0, count).Select(index => (double?)index).ToList();
        var input = new QualityInput(count, elapsed, new Dictionary<string, IReadOnlyList<double?>>(), texts, [], false);
        var quality = new QualityResult(count, [], false, false, null, null, null, [], [], null, [], [], QualityGrade.A, []);

        return new PhaseFrame(QualityTimeline.Of(input)!, quality);
    }

    private static PhaseSegment SegmentAt(PhaseResult result, int index)
        => result.Segments.Single(segment => segment.StartIndex <= index && index <= segment.EndIndexInclusive);

    private static void AssertFullPartition(PhaseResult result)
        => Assert.Equal(result.SampleCount, result.Segments.Sum(segment => segment.EndIndexInclusive - segment.StartIndex + 1));

    private static int RowLine(int sampleIndex) => sampleIndex + 1;
}
