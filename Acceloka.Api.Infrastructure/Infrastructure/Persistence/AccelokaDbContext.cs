using Microsoft.EntityFrameworkCore;
using Acceloka.Api.Domain.Entities;

namespace Acceloka.Api.Infrastructure.Persistence
{
    public class AccelokaDbContext : DbContext
    {
        public AccelokaDbContext(DbContextOptions<AccelokaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Ticket> Tickets => Set<Ticket>();

        public DbSet<BookedTicket> BookedTickets => Set<BookedTicket>();

        public DbSet<BookedTicketDetail> BookedTicketDetails
            => Set<BookedTicketDetail>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 2);
        }
    }
}
