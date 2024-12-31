using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfigurations
{
    internal sealed class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.ToTable("budgets");
            builder.HasKey(b => b.Id);

            builder.HasMany(b => b.Expenses)
                  .WithOne(e => e.Budget)
                  .HasForeignKey(e => e.BudgetId);

            builder.HasMany(b => b.Incomes)
                  .WithOne(i => i.Budget)
                  .HasForeignKey(i => i.BudgetId);
        }
    }
}
