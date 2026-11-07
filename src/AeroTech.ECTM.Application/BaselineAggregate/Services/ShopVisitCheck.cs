using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Services
{
    public sealed class ShopVisitCheck
    {
        private readonly IMaintenanceEventRepository _maintenanceEvents;

        public ShopVisitCheck(IMaintenanceEventRepository maintenanceEvents) => _maintenanceEvents = maintenanceEvents;

        public async Task EnsureAsync(long engineId, long? shopVisitEventId, CancellationToken cancellationToken = default)
        {
            if (shopVisitEventId is not { } eventId)
                return;

            var maintenanceEvent = await _maintenanceEvents.GetAsync(eventId, cancellationToken);

            if (maintenanceEvent is null)
                throw ExceptionFactory.MaintenanceEventNotFound(eventId);

            var belongsToEngine = maintenanceEvent.EngineId == engineId;
            var isShopVisit = maintenanceEvent.EventType == MaintenanceEventType.SHOP_VISIT;

            if (!belongsToEngine || !isShopVisit)
                throw ExceptionFactory.BaselineShopVisitInvalid(eventId, engineId);
        }
    }
}
