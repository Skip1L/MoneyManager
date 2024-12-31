using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfigurations
{
    internal sealed class IncomeConfiguration : IEntityTypeConfiguration<Income>
    {
        public void Configure(EntityTypeBuilder<Income> builder)
        {
            builder.ToTable("incomes");
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.Category)
                .WithMany(c => c.Incomes)
                .HasForeignKey(i => i.CategoryId);

            builder.HasOne(i => i.Budget)
                .WithMany(b => b.Incomes)
                .HasForeignKey(i => i.BudgetId);
        }
    }
}
