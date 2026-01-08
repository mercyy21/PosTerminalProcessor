using PosTerminalProcessor.Application.DataTransferObject;
using PosTerminalProcessor.Application.Terminals.Dtos;
using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.Application.Interfaces
{
    public interface ITerminalService
    {
        Task<Result> CreateTerminal(CreateTerminalRequest request, string accessToken, CancellationToken cancellationToken);
        Task<Result> GetTerminalDetails(GetTerminalQueryRequest queryRequest, string accessToken, CancellationToken cancellationToken);

    }
}
