using AeroTech.Framework.Core.Domain.Exceptions;

namespace AeroTech.ECTM.Domain._Shared.Resources
{
    public static class ExceptionFactory
    {
        // Configuration registry: 80100-80199
        public static BusinessException ConfigurationKindNotFound(params object?[] args) =>
            new(80100, ExceptionMessages.ConfigurationKindNotFound, args) { HttpStatus = 404 };

        public static BusinessException ConfigurationDocumentNotFound(params object?[] args) =>
            new(80101, ExceptionMessages.ConfigurationDocumentNotFound, args) { HttpStatus = 404 };

        public static BusinessException ConfigurationDocumentVersionIsInvalid(params object?[] args) =>
            new(80102, ExceptionMessages.ConfigurationDocumentVersionIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException ConfigurationDocumentContentIsRequired() =>
            new(80103, ExceptionMessages.ConfigurationDocumentContentIsRequired) { HttpStatus = 422 };

        public static BusinessException ConfigurationDocumentContentIsNotJsonObject() =>
            new(80104, ExceptionMessages.ConfigurationDocumentContentIsNotJsonObject) { HttpStatus = 422 };
    }
}
