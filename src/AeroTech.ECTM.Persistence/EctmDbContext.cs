using AeroTech.ECTM.Domain.AuditEntryAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Persistence.AuditEntryAggregate;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Framework.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence
{
    public sealed class EctmDbContext : CommandDbContext, IUnitOfWork
    {
        public const string DefaultSchema = "Ectm";
        public const string MigrationsHistorySchema = "dbo";
        public const string MigrationsHistoryTable = "__CommandsMigrationHistory";

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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            RecordAuditEntries();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            RecordAuditEntries();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
            base.OnModelCreating(modelBuilder);
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
