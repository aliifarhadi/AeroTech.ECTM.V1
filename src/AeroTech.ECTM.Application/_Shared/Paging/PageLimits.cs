namespace AeroTech.ECTM.Application._Shared.Paging
{
    public static class PageLimits
    {
        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 100;
        public const int MaxPageNumber = int.MaxValue / MaxPageSize;
    }
}
