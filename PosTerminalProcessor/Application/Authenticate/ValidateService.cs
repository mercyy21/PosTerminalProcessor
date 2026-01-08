using PosTerminalProcessor.Application.Interfaces;
using RestSharp;
using System.Net;

namespace PosTerminalProcessor.Application.Authenticate
{
    public class ValidateService : IValidateService
    {
        private readonly IRestClient _client;
        private readonly IConfiguration _configuration;
        public ValidateService(IRestClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }
        public async Task<ValidateResponse> ValidateToken(string apiKey)
        {
            string validateApiUrl = _configuration["ValidateApiUrl"];
            ValidateResponse response = await Get<ValidateResponse>(validateApiUrl, apiKey);
            return response;
        }
        private async Task<T> Get<T>(string apiUrl, string apiKey)
        {
            try
            {
                RestRequest restRequest = new(apiUrl);
                if (!string.IsNullOrEmpty(apiKey))
                {
                    restRequest.AddHeader("Accept", "application/json");
                    restRequest.AddHeader("Authorization", apiKey);
                }
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                var response = await _client.GetAsync<T>(restRequest);
                return response;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
