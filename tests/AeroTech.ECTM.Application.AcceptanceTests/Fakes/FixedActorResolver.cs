using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fakes;

public sealed class FixedActorResolver(Actor actor) : IActorResolver
{
    public static readonly FixedActorResolver Alice = new(new Actor("alice", 42, "user"));

    public Actor Resolve() => actor;
}
