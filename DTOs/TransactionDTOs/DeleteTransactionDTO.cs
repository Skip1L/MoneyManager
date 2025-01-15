using Domain.Enums;

namespace DTOs.TransactionDTOs
{
    public class DeleteTransactionDTO
    {
        public Guid Id { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
