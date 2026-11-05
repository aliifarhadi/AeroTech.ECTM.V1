using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AeroTech.ECTM.Persistence._Shared
{
    internal static class ImmutableBytes
    {
        public static readonly ValueComparer<byte[]> Comparer = new(
            (left, right) => ReferenceEquals(left, right),
            value => RuntimeHelpers.GetHashCode(value),
            value => value);
    }
}
