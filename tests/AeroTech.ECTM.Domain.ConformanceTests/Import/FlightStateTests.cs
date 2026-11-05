using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain.ConformanceTests.Fakes;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class FlightStateTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 5, 8, 0, 0, TimeSpan.Zero);

    private readonly SequenceIds _ids = new();

    [Fact]
    public void A_resolved_flight_waits_for_review_and_an_unresolved_one_for_identity()
    {
        Assert.Equal(FlightState.PENDING_REVIEW, FlightWith(IdentityStatus.RESOLVED).State);
        Assert.Equal(FlightState.NEEDS_IDENTITY, FlightWith(IdentityStatus.UNRESOLVED).State);
        Assert.Equal(FlightState.NEEDS_IDENTITY, FlightWith(IdentityStatus.NEEDS_CONFIRMATION).State);
    }

    [Fact]
    public void Hold_and_release_return_to_the_earlier_state()
    {
        var flight = FlightWith(IdentityStatus.RESOLVED);

        flight.Review(ReviewAction.HOLD, " waiting for the engine shop ", Now);

        Assert.Equal(FlightState.HELD, flight.State);
        Assert.Equal("waiting for the engine shop", flight.HoldNote);

        flight.Review(ReviewAction.RELEASE, null, Now);

        Assert.Equal(FlightState.PENDING_REVIEW, flight.State);
        Assert.Null(flight.HoldNote);
    }

    [Fact]
    public void Request_identity_sends_a_flight_back_to_identity()
    {
        var flight = FlightWith(IdentityStatus.RESOLVED);

        flight.Review(ReviewAction.REQUEST_IDENTITY, "wrong tail?", Now);

        Assert.Equal(FlightState.NEEDS_IDENTITY, flight.State);
        Assert.Equal(IdentityStatus.NEEDS_CONFIRMATION, flight.IdentityStatus);
        Assert.Contains(IdentityReasons.IdentityReviewRequested, flight.IdentityReasons);
    }

    [Theory]
    [InlineData(ReviewAction.ACCEPT, FlightState.ACCEPTED)]
    [InlineData(ReviewAction.EXCLUDE, FlightState.EXCLUDED)]
    public void Accept_and_exclude_decide_a_pending_flight_and_a_new_revision_returns_it_to_review(ReviewAction action, FlightState decided)
    {
        var flight = FlightWith(IdentityStatus.RESOLVED);

        flight.Review(action, "reviewed", Now);
        Assert.Equal(decided, flight.State);

        flight.ReturnToReview();
        Assert.Equal(FlightState.PENDING_REVIEW, flight.State);
    }

    [Fact]
    public void Holding_an_accepted_flight_withdraws_the_acceptance_and_release_returns_it_to_pending_review()
    {
        var flight = FlightWith(IdentityStatus.RESOLVED);

        flight.Review(ReviewAction.ACCEPT, null, Now);
        flight.Review(ReviewAction.HOLD, "shop report", Now);
        flight.Review(ReviewAction.RELEASE, null, Now);

        Assert.Equal(FlightState.PENDING_REVIEW, flight.State);
    }

    [Fact]
    public void A_flight_that_needs_identity_cannot_be_accepted()
    {
        var exception = Assert.Throws<BusinessException>(() => FlightWith(IdentityStatus.UNRESOLVED).Review(ReviewAction.ACCEPT, null, Now));

        Assert.Equal(80314, exception.Code);
    }

    [Fact]
    public void A_transition_the_state_does_not_allow_is_refused()
    {
        var flight = FlightWith(IdentityStatus.RESOLVED);

        var release = Assert.Throws<BusinessException>(() => flight.Review(ReviewAction.RELEASE, null, Now));
        flight.Review(ReviewAction.HOLD, null, Now);
        var identity = Assert.Throws<BusinessException>(() => flight.ConfirmIdentity(IdentityOf(IdentityStatus.RESOLVED), false, null));

        Assert.Equal(80314, release.Code);
        Assert.Equal(80314, identity.Code);
    }

    [Fact]
    public void Confirming_the_identity_resolves_the_flight_and_updates_the_engines_in_place()
    {
        var flight = FlightWith(IdentityStatus.UNRESOLVED);
        var assignmentIds = flight.Engines.Select(engine => engine.Id).ToList();

        flight.ConfirmIdentity(IdentityOf(IdentityStatus.RESOLVED), true, "checked against the tech log");

        Assert.Equal(FlightState.PENDING_REVIEW, flight.State);
        Assert.True(flight.DistinctFlightConfirmed);
        Assert.Equal(assignmentIds, flight.Engines.Select(engine => engine.Id));
        Assert.All(flight.Engines, engine => Assert.Equal(EngineAssignmentStatus.ENGINE, engine.Status));
    }

    private Flight FlightWith(IdentityStatus status)
    {
        var versions = new ConfigurationVersions(1, 1, 2, 1);
        var recording = new FlightRecordingArgs(10, 20, 30, versions, "ELAPSED_S", 3, 0, 2);

        return Flight.Import(_ids.NewId(), recording, IdentityOf(status), [], [], _ids, Now);
    }

    private static IdentityResolution IdentityOf(IdentityStatus status)
    {
        var assignment = status == IdentityStatus.RESOLVED ? EngineAssignmentStatus.ENGINE : EngineAssignmentStatus.NEEDS_CONFIRMATION;
        long? engineOne = status == IdentityStatus.RESOLVED ? 501 : null;
        long? engineTwo = status == IdentityStatus.RESOLVED ? 502 : null;

        return new IdentityResolution(
            100,
            "EP-DOA",
            IdentitySource.FILE_NAME,
            new DateOnly(2026, 10, 1),
            IdentitySource.FILE_NAME,
            "25",
            IdentitySource.FILE_NAME,
            status,
            status == IdentityStatus.RESOLVED ? [] : [IdentityReasons.AircraftNotFound],
            [new EngineResolution(1, assignment, engineOne, null), new EngineResolution(2, assignment, engineTwo, null)]);
    }
}
