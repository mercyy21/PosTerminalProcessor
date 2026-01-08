using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTerminalProcessor.Application.Customers.Dtos;
using PosTerminalProcessor.Application.Interfaces;
using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class CustomerController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly string accessToken;
        public CustomerController(IHttpContextAccessor httpContextAccessor, ICustomerService customerService)
        {
            _httpContextAccessor = httpContextAccessor;
            _customerService = customerService;

            accessToken = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new UnauthorizedAccessException("You are not authorized!");
            }
        }

        /// <summary>
        /// Create Customer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<Result>> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.CreateCustomer(request, accessToken, cancellationToken));
        }
        /// <summary>
        /// Connect Customer To Terminal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("connectcustomertoterminal")]
        public async Task<ActionResult<Result>> ConnectCustomerToTerminal(CustomerToTerminalConnectorRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.ConnectCustomerToTerminal(request, accessToken, cancellationToken));
        }
        /// <summary>
        /// Get Customers Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("customersdetails")]
        public async Task<ActionResult<Result>> GetCustomers([FromQuery] GetCustomerQueryRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _customerService.GetCustomerDetails(request, accessToken, cancellationToken));
        }
    }
}
