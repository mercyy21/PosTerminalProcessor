using PosTerminalProcessor.Application.Authenticate;

namespace PosTerminalProcessor.Application.Interfaces
{
    public interface IValidateService
    {
        Task<ValidateResponse> ValidateToken(string apiKey);
    }
}
