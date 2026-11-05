using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.ECTM.Domain.ConformanceTests.Fakes;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class IdentityResolverTests
{
    private const long EngineOne = 501;
    private const long EngineTwo = 502;

    private static readonly DateTimeOffset September24 = new(2026, 9, 24, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Now = new(2026, 11, 5, 8, 0, 0, TimeSpan.Zero);
    private static readonly IReadOnlyList<int> BothPositions = [1, 2];

    private readonly SequenceIds _ids = new();
    private readonly Aircraft _aircraft = Aircraft.Register(100, "EP-DOA", null, null);

    [Fact]
    public void The_file_name_gives_date_registration_and_flight_number()
    {
        var identity = FileNameIdentity.Parse("2026-10-01_ep-doa_25.CSV");

        Assert.Equal(new FileNameIdentity(new DateOnly(2026, 10, 1), "EP-DOA", "25"), identity);
        Assert.Equal(new FileNameIdentity(null, null, null), FileNameIdentity.Parse("flight-25.csv"));
        Assert.Equal(new FileNameIdentity(null, null, null), FileNameIdentity.Parse("SYNTHETIC_TEST_ONLY_2026-10-01_EP-DOA_25.csv"));
    }

    [Fact]
    public void Both_engines_come_from_the_installations_covering_the_whole_flight_date()
    {
        var identity = Resolve(Recordings.Flight25, Installations(), _aircraft);

        Assert.Equal(IdentityStatus.RESOLVED, identity.Status);
        Assert.Empty(identity.Reasons);
        Assert.Equal((new DateOnly(2026, 10, 1), "25"), (identity.FlightDate!.Value, identity.FlightNumber!));
        Assert.Equal(IdentitySource.FILE_NAME, identity.FlightDateSource);
        Assert.Equal([EngineOne, EngineTwo], identity.Engines.Select(engine => engine.EngineId!.Value));
        Assert.All(identity.Engines, engine => Assert.Equal(EngineAssignmentStatus.ENGINE, engine.Status));
    }

    [Fact]
    public void An_unknown_registration_is_unresolved_and_no_engine_is_guessed()
    {
        var identity = Resolve("2026-10-01_EP-XYZ_25.csv", Installations(), null);

        Assert.Equal(IdentityStatus.UNRESOLVED, identity.Status);
        Assert.Contains(IdentityReasons.AircraftNotFound, identity.Reasons);
        Assert.All(identity.Engines, engine => Assert.Null(engine.EngineId));
    }

    [Fact]
    public void A_position_without_an_installation_is_position_only()
    {
        var onlyEngineOne = Installations().Where(installation => installation.Position == 1).ToList();

        var identity = Resolve(Recordings.Flight25, onlyEngineOne, _aircraft);

        Assert.Equal(IdentityStatus.NEEDS_CONFIRMATION, identity.Status);
        Assert.Equal([IdentityReasons.NoInstallation(2)], identity.Reasons);
        Assert.Equal(EngineAssignmentStatus.POSITION_ONLY, identity.Engines.Single(engine => engine.Position == 2).Status);
    }

    [Fact]
    public void An_installation_change_on_the_flight_date_needs_confirmation()
    {
        var installations = Installations();
        installations[1].Close(new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero), "removed for test", _ids, Now);

        var identity = Resolve(Recordings.Flight25, installations, _aircraft);

        Assert.Equal(IdentityStatus.NEEDS_CONFIRMATION, identity.Status);
        Assert.Equal([IdentityReasons.InstallationChanged(2)], identity.Reasons);
        Assert.Null(identity.Engines.Single(engine => engine.Position == 2).EngineId);
    }

    [Fact]
    public void Columns_that_disagree_with_the_file_name_need_confirmation_and_the_name_wins()
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);
        var claim = IdentityClaims.From("2026-10-02_EP-DOA_26.csv", null, null, recording);

        Assert.Equal([IdentityReasons.FlightNumberMismatch, IdentityReasons.DayMismatch], claim.Reasons);
        Assert.Equal(("26", new DateOnly(2026, 10, 2)), (claim.FlightNumber!, claim.FlightDate!.Value));
    }

    [Fact]
    public void Defaults_fill_what_the_file_name_does_not_give()
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);
        var claim = IdentityClaims.From("export.csv", " ep-doa ", 2026, recording);

        Assert.Empty(claim.Reasons);
        Assert.Equal(("EP-DOA", IdentitySource.USER_DEFAULT), (claim.Registration!, claim.RegistrationSource!.Value));
        Assert.Equal((new DateOnly(2026, 10, 1), IdentitySource.COLUMNS), (claim.FlightDate!.Value, claim.FlightDateSource!.Value));
        Assert.Equal(("25", IdentitySource.COLUMNS), (claim.FlightNumber!, claim.FlightNumberSource!.Value));
    }

    [Fact]
    public void A_possible_duplicate_identity_needs_confirmation()
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);
        var claim = IdentityClaims.From(Recordings.Flight25, null, null, recording);

        var identity = IdentityResolver.Resolve(claim, _aircraft, Installations(), BothPositions, possibleDuplicate: true);

        Assert.Equal(IdentityStatus.NEEDS_CONFIRMATION, identity.Status);
        Assert.Equal([IdentityReasons.PossibleDuplicateIdentity], identity.Reasons);
    }

    [Fact]
    public void The_user_confirms_an_installed_engine_or_position_only()
    {
        var identity = IdentityResolver.ConfirmByUser(
            _aircraft,
            new DateOnly(2026, 10, 1),
            " 25 ",
            [new EngineChoiceArgs(1, EngineOne, false), new EngineChoiceArgs(2, null, true)],
            BothPositions,
            Installations());

        Assert.Equal(IdentityStatus.RESOLVED, identity.Status);
        Assert.Equal(IdentitySource.USER, identity.FlightNumberSource);
        Assert.Equal("25", identity.FlightNumber);
        Assert.Equal(EngineAssignmentStatus.POSITION_ONLY, identity.Engines.Single(engine => engine.Position == 2).Status);
    }

    [Fact]
    public void The_user_cannot_confirm_an_engine_that_was_not_installed_there_that_day()
    {
        var exception = Assert.Throws<BusinessException>(() => IdentityResolver.ConfirmByUser(
            _aircraft,
            new DateOnly(2026, 9, 1),
            "25",
            [new EngineChoiceArgs(1, EngineOne, false), new EngineChoiceArgs(2, EngineTwo, false)],
            BothPositions,
            Installations()));

        Assert.Equal(80312, exception.Code);
    }

    [Fact]
    public void Every_recorded_position_needs_exactly_one_choice()
    {
        var exception = Assert.Throws<BusinessException>(() => IdentityResolver.ConfirmByUser(
            _aircraft,
            new DateOnly(2026, 10, 1),
            "25",
            [new EngineChoiceArgs(1, EngineOne, false)],
            BothPositions,
            Installations()));

        Assert.Equal(80309, exception.Code);
    }

    private IdentityResolution Resolve(string fileName, IReadOnlyList<EngineInstallation> installations, Aircraft? aircraft)
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);
        var claim = IdentityClaims.From(fileName, null, null, recording);

        return IdentityResolver.Resolve(claim, aircraft, installations, recording.EnginePositions, possibleDuplicate: false);
    }

    private List<EngineInstallation> Installations()
        =>
        [
            EngineInstallation.Record(_ids.NewId(), EngineOne, _aircraft.Id, 1, September24, "initial installation", null, _ids, Now),
            EngineInstallation.Record(_ids.NewId(), EngineTwo, _aircraft.Id, 2, September24, "initial installation", null, _ids, Now)
        ];
}
