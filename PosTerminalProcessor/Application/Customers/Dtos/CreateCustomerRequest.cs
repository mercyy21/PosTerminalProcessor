using System.ComponentModel.DataAnnotations;

namespace PosTerminalProcessor.Application.Customers.Dtos
{
    public class CreateCustomerRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
