using JerseyStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JerseyStore.Api.Data.Configurations;

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url).HasMaxLength(2048).IsRequired();
        builder.Property(i => i.AltText).HasMaxLength(200);

        builder.HasIndex(i => new { i.ProductId, i.DisplayOrder });
        builder.HasIndex(i => new { i.ProductId, i.IsPrimary });
    }
}
