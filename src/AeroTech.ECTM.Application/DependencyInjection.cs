using AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds;
using AeroTech.ECTM.Application._Shared.Behaviors;
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

            services.AddScoped<IGetConfigurationKindsService, GetConfigurationKindsService>();
            services.AddScoped<IGetConfigurationDocumentsPaginatedService, GetConfigurationDocumentsPaginatedService>();
            services.AddScoped<IGetConfigurationDocumentByIdService, GetConfigurationDocumentByIdService>();

            services.AddScoped<IGetAuditEntriesPaginatedService, GetAuditEntriesPaginatedService>();

            return services;
        }
    }
}
