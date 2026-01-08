using Microsoft.EntityFrameworkCore;
using PosTerminalProcessor.Domain;

namespace PosTerminalProcessor.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; set; }
        DbSet<Terminal> Terminals { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
