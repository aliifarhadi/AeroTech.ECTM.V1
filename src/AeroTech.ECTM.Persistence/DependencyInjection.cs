using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.AuditEntryAggregate.Contracts;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.ECTM.Persistence.AircraftAggregate;
using AeroTech.ECTM.Persistence.AnalysisRevisionAggregate;
using AeroTech.ECTM.Persistence.AuditEntryAggregate;
using AeroTech.ECTM.Persistence.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Persistence.EngineAggregate;
using AeroTech.ECTM.Persistence.EngineInstallationAggregate;
using AeroTech.ECTM.Persistence.EngineModelAggregate;
using AeroTech.ECTM.Persistence.FlightAggregate;
using AeroTech.ECTM.Persistence.FlightPhaseAssessmentAggregate;
using AeroTech.ECTM.Persistence.FlightQualityAggregate;
using AeroTech.ECTM.Persistence.ImportBatchAggregate;
using AeroTech.ECTM.Persistence.MaintenanceEventAggregate;
using AeroTech.ECTM.Persistence.ProcessingJobAggregate;
using AeroTech.ECTM.Persistence.SourceFileAggregate;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Infrastructure.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AeroTech.ECTM.Persistence
{
    public static class DependencyInjection
    {
        public const string ConnectionStringName = "CommandDbContext";

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, $"ConnectionStrings:{ConnectionStringName}");

            services.AddDbContext<EctmDbContext>(options => options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsHistoryTable(EctmDbContext.MigrationsHistoryTable, EctmDbContext.MigrationsHistorySchema)));
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EctmDbContext>());

            services.AddScoped<IConfigurationDocumentRepository, ConfigurationDocumentRepository>();
            services.AddScoped<IAuditEntryRepository, AuditEntryRepository>();

            services.AddScoped<IAircraftRepository, AircraftRepository>();
            services.AddScoped<IEngineModelRepository, EngineModelRepository>();
            services.AddScoped<IEngineRepository, EngineRepository>();
            services.AddScoped<IEngineInstallationRepository, EngineInstallationRepository>();
            services.AddScoped<IMaintenanceEventRepository, MaintenanceEventRepository>();

            services.AddScoped<ISourceFileRepository, SourceFileRepository>();
            services.AddScoped<IImportBatchRepository, ImportBatchRepository>();
            services.AddScoped<IFlightRepository, FlightRepository>();
            services.AddScoped<IProcessingJobRepository, ProcessingJobRepository>();
            services.AddScoped<IFlightQualityRepository, FlightQualityRepository>();
            services.AddScoped<IAnalysisRevisionRepository, AnalysisRevisionRepository>();
            services.AddScoped<IFlightPhaseAssessmentRepository, FlightPhaseAssessmentRepository>();

            services.AddHealthChecks().AddDbContextReadinessCheck<EctmDbContext>("sql-server-command");

            return services;
        }
    }
}
