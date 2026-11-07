using Microsoft.AspNetCore.Http;

namespace AeroTech.ECTM.RestApi.V1.BaselineAggregate.Requests
{
    public sealed class PrefillBaselineFileRequest
    {
        public IFormFile? File { get; set; }

        public int EnginePosition { get; set; }

        public long? ShopVisitEventId { get; set; }
    }
}
