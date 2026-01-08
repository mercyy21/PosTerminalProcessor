using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.Domain
{
    public class Terminal : BaseEntity
    {
        public string TerminalNumber { get; set; }
        public string TerminalGate { get; set; }
        public int CustomerId { get; set; }
    }
}
