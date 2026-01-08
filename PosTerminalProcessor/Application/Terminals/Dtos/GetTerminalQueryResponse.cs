using PosTerminalProcessor.Application.Customers.Dtos;

namespace PosTerminalProcessor.Application.Terminals.Dtos
{
    public class GetTerminalQueryResponse
    {
        public string TerminalNumber { get; set; }
        public string TerminalGate { get; set; }
        public GetCustomerQueryResponse Customer { get; set; }
    }
}
