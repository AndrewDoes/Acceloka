using Acceloka.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AccelokaDbContext context)
        {
            // make sure DB exists
            await context.Database.MigrateAsync();

            // ❌ already seeded? then stop
            if (context.Categories.Any())
                return;

            // ===== CATEGORIES =====
            var concert = new Category { Name = "Concert" };
            var sport = new Category { Name = "Sport" };

            context.Categories.AddRange(concert, sport);
            await context.SaveChangesAsync();

            // ===== TICKETS =====
            var ticket1 = new Ticket
            {
                KodeTiket = "C-001",
                NamaTiket = "Coldplay VIP",
                Harga = 5_000_000,
                EventDate = new DateTime(2026, 6, 1),
                Quota = 100,
                CategoryId = concert.Id
            };

            var ticket2 = new Ticket
            {
                KodeTiket = "S-001",
                NamaTiket = "Football Regular",
                Harga = 250_000,
                EventDate = new DateTime(2026, 7, 10),
                Quota = 500,
                CategoryId = sport.Id
            };

            context.Tickets.AddRange(ticket1, ticket2);
            await context.SaveChangesAsync();

            // ===== BOOKED TICKET =====
            var bookedTicket = new BookedTicket
            {
                BookingDate = DateTime.UtcNow
            };

            context.BookedTickets.Add(bookedTicket);
            await context.SaveChangesAsync();

            // ===== BOOKED TICKET DETAILS =====
            var detail1 = new BookedTicketDetail
            {
                BookedTicketId = bookedTicket.Id,
                TicketId = ticket1.Id,
                Quantity = 2
            };

            var detail2 = new BookedTicketDetail
            {
                BookedTicketId = bookedTicket.Id,
                TicketId = ticket2.Id,
                Quantity = 3
            };

            context.BookedTicketDetails.AddRange(detail1, detail2);
            await context.SaveChangesAsync();
        }
    }
}
