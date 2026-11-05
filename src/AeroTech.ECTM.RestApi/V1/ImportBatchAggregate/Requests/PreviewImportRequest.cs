using Microsoft.AspNetCore.Http;

namespace AeroTech.ECTM.RestApi.V1.ImportBatchAggregate.Requests
{
    public sealed class PreviewImportRequest
    {
        public List<IFormFile> Files { get; set; } = new();

        public string? DefaultRegistration { get; set; }

        public int? DefaultYear { get; set; }
    }
}
