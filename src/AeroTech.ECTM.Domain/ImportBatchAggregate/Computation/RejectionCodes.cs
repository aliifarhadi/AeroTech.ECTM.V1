namespace AeroTech.ECTM.Domain.ImportBatchAggregate.Computation
{
    public static class RejectionCodes
    {
        public const string ProfileSignatureMismatch = "PROFILE_SIGNATURE_MISMATCH";
        public const string UnsupportedFileType = "UNSUPPORTED_FILE_TYPE";
        public const string NoDataRows = "NO_DATA_ROWS";
        public const string SyntheticSourceNotAllowed = "SYNTHETIC_SOURCE_NOT_ALLOWED";
        public const string SourceFileUnreadable = "SOURCE_FILE_UNREADABLE";
        public const string FileTooLarge = "FILE_TOO_LARGE";
    }
}
