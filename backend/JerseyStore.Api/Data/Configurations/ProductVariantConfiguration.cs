using JerseyStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JerseyStore.Api.Data.Configurations;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Size).HasMaxLength(20).IsRequired();
        builder.Property(v => v.Price).HasColumnType("numeric(18,2)").HasPrecision(18, 2);
        builder.Property(v => v.Sku).HasMaxLength(64);

        builder.HasIndex(v => new { v.ProductId, v.Size }).IsUnique();
        builder.HasIndex(v => new { v.ProductId, v.SizeOrder });
        builder.HasIndex(v => v.IsActive);
    }
}
