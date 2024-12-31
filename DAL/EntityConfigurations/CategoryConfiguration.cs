using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfigurations
{
    internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.HasKey(u => u.Id);

            builder.HasMany(c => c.Expenses)
               .WithOne(c => c.Category)
               .HasForeignKey(i => i.CategoryId);

            builder.HasMany(c => c.Incomes)
               .WithOne(c => c.Category)
               .HasForeignKey(i => i.CategoryId);
        }
    }
}
