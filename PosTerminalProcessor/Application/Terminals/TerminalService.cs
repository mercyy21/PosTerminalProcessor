using Microsoft.EntityFrameworkCore;
using PosTerminalProcessor.Application.Authenticate;
using PosTerminalProcessor.Application.Customers.Dtos;
using PosTerminalProcessor.Application.DataTransferObject;
using PosTerminalProcessor.Application.Interfaces;
using PosTerminalProcessor.Application.Terminals.Dtos;
using PosTerminalProcessor.Domain;
using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.Application.Terminals
{
    public class TerminalService : ITerminalService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidateService _validateService;
        public TerminalService(IApplicationDbContext context, IValidateService validateService)
        {
            _context = context;
            _validateService = validateService;
        }

        public async Task<Result> CreateTerminal(CreateTerminalRequest request, string accessToken, CancellationToken cancellationToken)
        {
            //Validation
            ValidateResponse validatonResult = await _validateService.ValidateToken(accessToken);
            if (!validatonResult.Valid)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Invalid user."
                };
            }
            if (request == null)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Invalid terminal data"
                };
            }
            Terminal existingTerminal = await _context.Terminals.FirstOrDefaultAsync(x => x.TerminalNumber.ToLower() == request.TerminalNumber.ToLower(), cancellationToken);
            if (existingTerminal != null)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = $"Terminal with number, {request.TerminalNumber}, already exists"
                };
            }
            Terminal terminal = new()
            {
                TerminalGate = request.TerminalGate,
                TerminalNumber = request.TerminalNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = validatonResult.Username
            };

            await _context.Terminals.AddAsync(terminal, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Result { ResponseCode = "00", Description = "Success", Message = "Terminal created successfully" };
        }

        public async Task<Result> GetTerminalDetails(GetTerminalQueryRequest queryRequest, string accessToken, CancellationToken cancellationToken)
        {
            ValidateResponse validatonResult = await _validateService.ValidateToken(accessToken);
            if (!validatonResult.Valid)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Invalid user."
                };
            }
            var terminals = _context.Terminals.AsNoTracking().GroupJoin(_context.Customers.AsNoTracking(), terminal => terminal.CustomerId, customer => customer.Id, (terminal, customers) => new { terminal, customers }).SelectMany(x => x.customers.DefaultIfEmpty(), (x, customer) => new { x.terminal, customer });

            if (queryRequest.Id.HasValue)
                terminals = terminals.Where(x => x.terminal.Id == queryRequest.Id.Value);

            if (!string.IsNullOrWhiteSpace(queryRequest.TerminalNumber))
                terminals = terminals.Where(x => x.terminal.TerminalNumber.Contains(queryRequest.TerminalNumber));

            if (!string.IsNullOrWhiteSpace(queryRequest.TerminalGate))
                terminals = terminals.Where(x => x.terminal.TerminalGate.Contains(queryRequest.TerminalGate));

            List<GetTerminalQueryResponse> result = await terminals
                .OrderByDescending(x => x.terminal.CreatedAt)
                .Select(x => new GetTerminalQueryResponse
                {
                    TerminalGate = x.terminal.TerminalGate,
                    TerminalNumber = x.terminal.TerminalNumber,
                    Customer = x.customer != null ? new GetCustomerQueryResponse
                    {
                        FirstName = x.customer.FirstName,
                        LastName = x.customer.LastName,
                        Email = x.customer.Email,
                        PhoneNumber = x.customer.PhoneNumber
                    } : null

                }).ToListAsync(cancellationToken);
            return new Result
            {
                ResponseCode = "00",
                Description = "Success",
                Message = "Customer(s) retrieved successfully",
                Data = result
            };
        }
    }
}
