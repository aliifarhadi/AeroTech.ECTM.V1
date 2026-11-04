using AeroTech.ECTM.Domain.AuditEntryAggregate.Contracts;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.ECTM.Persistence.AuditEntryAggregate;
using AeroTech.ECTM.Persistence.ConfigurationDocumentAggregate;
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

            services.AddHealthChecks().AddDbContextReadinessCheck<EctmDbContext>("sql-server-command");

            return services;
        }
    }
}
