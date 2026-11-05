using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.ECTM.Domain.AuditEntryAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.EngineAggregate;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.ECTM.Domain.EngineModelAggregate;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate;
using AeroTech.ECTM.Domain.ImportBatchAggregate;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.SourceFileAggregate;
using AeroTech.ECTM.Persistence.AuditEntryAggregate;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Framework.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence
{
    public sealed class EctmDbContext : CommandDbContext, IUnitOfWork
    {
        public const string DefaultSchema = "Ectm";
        public const string RegistryVersionSequence = "RegistryVersions";
        public const string MigrationsHistorySchema = "dbo";
        public const string MigrationsHistoryTable = "__CommandsMigrationHistory";

        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        private readonly IActorResolver _actorResolver;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;

        public EctmDbContext(
            DbContextOptions<EctmDbContext> options,
            IActorResolver actorResolver,
            IClock clock,
            IDomainEventDispatcher domainEventDispatcher,
            IIdGenerator idGenerator)
            : base(options, actorResolver, clock, domainEventDispatcher)
        {
            _actorResolver = actorResolver;
            _clock = clock;
            _idGenerator = idGenerator;
        }

        public DbSet<ConfigurationDocument> ConfigurationDocuments => Set<ConfigurationDocument>();

        public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

        public DbSet<Aircraft> Aircraft => Set<Aircraft>();

        public DbSet<EngineModel> EngineModels => Set<EngineModel>();

        public DbSet<Engine> Engines => Set<Engine>();

        public DbSet<EngineInstallation> EngineInstallations => Set<EngineInstallation>();

        public DbSet<MaintenanceEvent> MaintenanceEvents => Set<MaintenanceEvent>();

        public DbSet<SourceFile> SourceFiles => Set<SourceFile>();

        public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();

        public DbSet<Flight> Flights => Set<Flight>();

        public DbSet<ProcessingJob> ProcessingJobs => Set<ProcessingJob>();

        public DbSet<FlightQuality> FlightQualities => Set<FlightQuality>();

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            RecordAuditEntries();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            RecordAuditEntries();

            try
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw ExceptionFactory.ResourceChangedConcurrently();
            }
            catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation })
            {
                throw ExceptionFactory.ResourceChangedConcurrently();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasSequence<long>(RegistryVersionSequence);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EctmDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
            configurationBuilder.Properties<string>().HaveMaxLength(256);
        }

        private void RecordAuditEntries()
        {
            var actor = _actorResolver.Resolve();
            var occurredAt = _clock.GetDateTime();
            var auditEntries = AuditEntryRecorder.Record(ChangeTracker, actor, occurredAt, _idGenerator);

            AuditEntries.AddRange(auditEntries);
        }
    }
}
