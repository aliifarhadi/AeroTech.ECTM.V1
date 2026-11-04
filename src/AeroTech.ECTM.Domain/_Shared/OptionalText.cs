namespace AeroTech.ECTM.Domain._Shared
{
    public static class OptionalText
    {
        public static string? Normalize(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return text.Trim();
        }
    }
}
