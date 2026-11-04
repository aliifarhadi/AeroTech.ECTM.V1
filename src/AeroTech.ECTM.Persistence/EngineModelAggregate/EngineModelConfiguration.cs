using AeroTech.ECTM.Domain.EngineModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.ECTM.Persistence.EngineModelAggregate
{
    public sealed class EngineModelConfiguration : IEntityTypeConfiguration<EngineModel>
    {
        public void Configure(EntityTypeBuilder<EngineModel> builder)
        {
            builder.ToTable("EngineModels");
            builder.HasKey(engineModel => engineModel.Id);
            builder.Property(engineModel => engineModel.Id).ValueGeneratedNever();

            builder.Property(engineModel => engineModel.Code)
                .HasMaxLength(64)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .IsRequired();
            builder.Property(engineModel => engineModel.Manufacturer).HasMaxLength(128).IsRequired();
            builder.Property(engineModel => engineModel.Family).HasMaxLength(64);
            builder.Property(engineModel => engineModel.VariantCode).HasMaxLength(64);
            builder.Property(engineModel => engineModel.ThrustRatingCode).HasMaxLength(64);
            builder.PrimitiveCollection(engineModel => engineModel.DocumentReferences).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(engineModel => engineModel.Notes).HasMaxLength(2000);

            builder.HasIndex(engineModel => engineModel.Code).IsUnique();
        }
    }
}
