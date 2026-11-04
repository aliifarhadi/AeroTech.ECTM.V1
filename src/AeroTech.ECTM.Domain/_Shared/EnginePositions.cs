using AeroTech.ECTM.Domain._Shared.Resources;

namespace AeroTech.ECTM.Domain._Shared
{
    public static class EnginePositions
    {
        public const byte First = 1;
        public const byte Second = 2;
        public const byte Count = 2;

        public static byte EnsureValid(int position)
        {
            if (position is not (First or Second))
                throw ExceptionFactory.EnginePositionIsInvalid(position);

            return (byte)position;
        }
    }
}
