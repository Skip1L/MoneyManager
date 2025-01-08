namespace DTOs.AuthorizationDTOs
{
    public class SuccessSignInDTO
    {
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
    }
}
