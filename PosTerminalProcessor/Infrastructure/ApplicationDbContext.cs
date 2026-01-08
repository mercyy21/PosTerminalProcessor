using Microsoft.EntityFrameworkCore;
using PosTerminalProcessor.Application.Interfaces;
using PosTerminalProcessor.Domain;

namespace PosTerminalProcessor.Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
