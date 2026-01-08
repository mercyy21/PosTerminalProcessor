using System.ComponentModel.DataAnnotations;

namespace PosTerminalProcessor.Application.Customers.Dtos
{
    public class CustomerToTerminalConnectorRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string TerminalNumber { get; set; }
    }
}
