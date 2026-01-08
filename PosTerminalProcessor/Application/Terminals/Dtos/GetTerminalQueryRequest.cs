namespace PosTerminalProcessor.Application.Terminals.Dtos
{
    public class GetTerminalQueryRequest
    {
        public int? Id { get; set; }
        public string TerminalNumber { get; set; }
        public string TerminalGate { get; set; }
    }
}
