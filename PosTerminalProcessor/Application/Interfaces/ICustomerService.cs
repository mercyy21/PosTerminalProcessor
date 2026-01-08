using PosTerminalProcessor.Application.Customers.Dtos;
using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Result> CreateCustomer(CreateCustomerRequest request, string accessToken, CancellationToken cancellationToken);
        Task<Result> GetCustomerDetails(GetCustomerQueryRequest queryRequest, string accessToken, CancellationToken cancellationToken);
        Task<Result> ConnectCustomerToTerminal(CustomerToTerminalConnectorRequest request, string accessToken, CancellationToken cancellationToken);
    }
}
