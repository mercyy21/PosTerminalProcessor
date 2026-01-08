using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTerminalProcessor.Application.DataTransferObject;
using PosTerminalProcessor.Application.Interfaces;
using PosTerminalProcessor.Application.Terminals.Dtos;
using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class TerminalController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITerminalService _terminalService;
        private readonly string accessToken;
        public TerminalController(IHttpContextAccessor httpContextAccessor, ITerminalService terminalService)
        {
            _httpContextAccessor = httpContextAccessor;
            _terminalService = terminalService;

            accessToken = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new UnauthorizedAccessException("You are not authorized!");
            }
        }

        /// <summary>
        /// Create Terminal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<Result>> CreateTerminal(CreateTerminalRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _terminalService.CreateTerminal(request, accessToken, cancellationToken));
        }
        /// <summary>
        /// Get Terminal Details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("terminaldetails")]
        public async Task<ActionResult<Result>> GetTerminals([FromQuery] GetTerminalQueryRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _terminalService.GetTerminalDetails(request, accessToken, cancellationToken));
        }
    }
}
