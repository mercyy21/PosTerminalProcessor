using Microsoft.EntityFrameworkCore;
using PosTerminalProcessor.Application.Authenticate;
using PosTerminalProcessor.Application.Customers.Dtos;
using PosTerminalProcessor.Application.Interfaces;
using PosTerminalProcessor.Domain;
using PosTerminalProcessor.Domain.Common;

namespace PosTerminalProcessor.Application.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidateService _validateService;
        public CustomerService(IApplicationDbContext context, IValidateService validateService)
        {
            _context = context;
            _validateService = validateService;
        }

        public async Task<Result> CreateCustomer(CreateCustomerRequest request, string accessToken, CancellationToken cancellationToken)
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
            if (request == null)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Invalid customer data"
                };
            }
            Customer existingCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.Email.ToLower() == request.Email.ToLower(), cancellationToken);
            if (existingCustomer != null)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = $"Customer with email,{request.Email}, already exists"
                };
            }

            Customer customer = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = validatonResult.Username
            };

            await _context.Customers.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Result { ResponseCode = "00", Description = "Success", Message = "Customer created successfully" };
        }

        public async Task<Result> GetCustomerDetails(GetCustomerQueryRequest queryRequest, string accessToken, CancellationToken cancellationToken)
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
            IQueryable<Customer> customers = _context.Customers.AsNoTracking();

            if (queryRequest.Id.HasValue)
                customers = customers.Where(x => x.Id == queryRequest.Id.Value);

            if (!string.IsNullOrWhiteSpace(queryRequest.FirstName))
                customers = customers.Where(x => x.FirstName.Contains(queryRequest.FirstName));

            if (!string.IsNullOrWhiteSpace(queryRequest.LastName))
                customers = customers.Where(x => x.LastName.Contains(queryRequest.LastName));

            if (!string.IsNullOrWhiteSpace(queryRequest.Email))
                customers = customers.Where(x => x.Email.Contains(queryRequest.Email));

            List<GetCustomerQueryResponse> result = await customers
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new GetCustomerQueryResponse
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                }).ToListAsync(cancellationToken);
            return new Result
            {
                ResponseCode = "00",
                Description = "Success",
                Message = "Customer(s) retrieved successfully",
                Data = result
            };
        }
        public async Task<Result> ConnectCustomerToTerminal(CustomerToTerminalConnectorRequest request, string accessToken, CancellationToken cancellationToken)
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
            Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);
            if (customer == null)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Customer not found"
                };
            }
            Terminal terminal = await _context.Terminals.FirstOrDefaultAsync(t => t.TerminalNumber == request.TerminalNumber, cancellationToken);
            if (terminal == null)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Terminal not found"
                };
            }
            if (terminal.CustomerId != 0)
            {
                return new Result
                {
                    ResponseCode = "05",
                    Description = "Failed",
                    Message = "Terminal is already assigned to a customer"
                };
            }
            terminal.CustomerId = customer.Id;
            await _context.SaveChangesAsync(cancellationToken);
            return new Result
            {
                ResponseCode = "00",
                Description = "Success",
                Message = "Customer connected to terminal successfully"
            };
        }
    }
}
