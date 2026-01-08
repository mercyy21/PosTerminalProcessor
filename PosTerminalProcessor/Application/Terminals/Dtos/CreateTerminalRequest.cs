using System.ComponentModel.DataAnnotations;

namespace PosTerminalProcessor.Application.DataTransferObject
{
    public class CreateTerminalRequest
    {
        [Required]
        public string TerminalNumber { get; set; }
        [Required]
        public string TerminalGate { get; set; }

    }
}
