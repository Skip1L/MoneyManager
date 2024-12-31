using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfigurations
{
    internal sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("expenses");
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(i => i.CategoryId);

            builder.HasOne(e => e.Budget)
                .WithMany(b =>  b.Expenses)
                .HasForeignKey(e =>  e.BudgetId);

        }

    }
}
