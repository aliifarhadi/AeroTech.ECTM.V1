using AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft;
using AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated;
using AeroTech.ECTM.Application.AircraftAggregate.Services;
using AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds;
using AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters;
using AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine;
using AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById;
using AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated;
using AeroTech.ECTM.Application.EngineAggregate.Services;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel;
using AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel;
using AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById;
using AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated;
using AeroTech.ECTM.Application.EngineModelAggregate.Services;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Services;
using AeroTech.ECTM.Application._Shared.Behaviors;
using AeroTech.ECTM.Application._Shared.Concurrency;
using AeroTech.ECTM.Application._Shared.Events;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.ServiceContracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AeroTech.ECTM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IValidator<>), typeof(PageQueryValidator<>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

            var resourceLock = configuration.GetSection(ResourceLockOptions.SectionName);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
                resourceLock.Get<ResourceLockOptions>()?.ExpirySeconds ?? 0,
                $"{ResourceLockOptions.SectionName}:{nameof(ResourceLockOptions.ExpirySeconds)}");
            services.Configure<ResourceLockOptions>(resourceLock);

            services.AddScoped<IGetConfigurationKindsService, GetConfigurationKindsService>();
            services.AddScoped<IGetConfigurationDocumentsPaginatedService, GetConfigurationDocumentsPaginatedService>();
            services.AddScoped<IGetConfigurationDocumentByIdService, GetConfigurationDocumentByIdService>();

            services.AddScoped<IGetAuditEntriesPaginatedService, GetAuditEntriesPaginatedService>();

            services.AddScoped<IAircraftLock, AircraftLock>();
            services.AddScoped<ICreateAircraftService, CreateAircraftService>();
            services.AddScoped<IUpdateAircraftService, UpdateAircraftService>();
            services.AddScoped<IGetAircraftByIdService, GetAircraftByIdService>();
            services.AddScoped<IGetAircraftPaginatedService, GetAircraftPaginatedService>();

            services.AddScoped<IEngineModelLock, EngineModelLock>();
            services.AddScoped<ICreateEngineModelService, CreateEngineModelService>();
            services.AddScoped<IUpdateEngineModelService, UpdateEngineModelService>();
            services.AddScoped<IGetEngineModelByIdService, GetEngineModelByIdService>();
            services.AddScoped<IGetEngineModelsPaginatedService, GetEngineModelsPaginatedService>();

            services.AddScoped<IEngineLock, EngineLock>();
            services.AddScoped<ICreateEngineService, CreateEngineService>();
            services.AddScoped<IUpdateEngineService, UpdateEngineService>();
            services.AddScoped<IAdjustEngineLifeCountersService, AdjustEngineLifeCountersService>();
            services.AddScoped<IGetEngineByIdService, GetEngineByIdService>();
            services.AddScoped<IGetEnginesPaginatedService, GetEnginesPaginatedService>();

            services.AddScoped<IEngineInstallationLock, EngineInstallationLock>();
            services.AddScoped<EngineInstallationDtoLoader>();
            services.AddScoped<ICreateEngineInstallationService, CreateEngineInstallationService>();
            services.AddScoped<ICloseEngineInstallationService, CloseEngineInstallationService>();
            services.AddScoped<IGetEngineInstallationsPaginatedService, GetEngineInstallationsPaginatedService>();

            services.AddScoped<MaintenanceEventDtoLoader>();
            services.AddScoped<ICreateMaintenanceEventService, CreateMaintenanceEventService>();
            services.AddScoped<IUpdateMaintenanceEventService, UpdateMaintenanceEventService>();
            services.AddScoped<IGetMaintenanceEventsPaginatedService, GetMaintenanceEventsPaginatedService>();

            return services;
        }
    }
}
