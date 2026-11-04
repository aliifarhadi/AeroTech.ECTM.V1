namespace AeroTech.ECTM.Domain._Shared.Resources
{
    public static class ExceptionMessages
    {
        // Configuration registry
        public const string ConfigurationKindNotFound = "Configuration kind {0} was not found.";
        public const string ConfigurationDocumentNotFound = "Configuration document {0} was not found.";
        public const string ConfigurationDocumentVersionIsInvalid = "Configuration document version {0} is invalid. Versions start at 1.";
        public const string ConfigurationDocumentContentIsRequired = "A configuration document requires content.";
        public const string ConfigurationDocumentContentIsNotJsonObject = "Configuration document content must be one JSON object.";
    }
}
