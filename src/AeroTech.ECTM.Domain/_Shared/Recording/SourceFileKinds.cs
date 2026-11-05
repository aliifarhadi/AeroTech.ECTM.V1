namespace AeroTech.ECTM.Domain._Shared.Recording
{
    public static class SourceFileKinds
    {
        public const string SyntheticMarker = "SYNTHETIC_TEST_ONLY";

        public static bool IsCsv(string fileName) => HasExtension(fileName, ".csv");

        public static bool IsXlsx(string fileName) => HasExtension(fileName, ".xlsx");

        public static bool IsZip(string fileName) => HasExtension(fileName, ".zip");

        public static bool IsRar(string fileName) => HasExtension(fileName, ".rar");

        public static bool IsRecording(string fileName) => IsCsv(fileName) || IsXlsx(fileName);

        public static bool IsArchive(string fileName) => IsZip(fileName) || IsRar(fileName);

        public static bool IsSynthetic(string fileName)
            => BareName(fileName).StartsWith(SyntheticMarker, StringComparison.OrdinalIgnoreCase);

        public static string BareName(string path) => Path.GetFileName(path.Replace('\\', '/'));

        private static bool HasExtension(string fileName, string extension)
            => string.Equals(Path.GetExtension(fileName), extension, StringComparison.OrdinalIgnoreCase);
    }
}
