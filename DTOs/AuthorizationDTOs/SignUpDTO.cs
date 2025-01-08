using System.ComponentModel.DataAnnotations;

namespace DTOs.AuthorizationDTOs
{
    public class SignUpDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
