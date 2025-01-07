namespace DTOs.AuthorizationDTOs
{
    public class SetLockoutEndDTO
    {
        public Guid UserId { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
    }

}
