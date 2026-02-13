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
            var festival = new Category { Name = "Festival" };
            var exhibition = new Category { Name = "Exhibition" };

            context.Categories.AddRange(concert, sport, festival, exhibition);
            await context.SaveChangesAsync();

            // ===== TICKETS =====
            var tickets = new List<Ticket>
            {
                // CONCERTS
                new Ticket { KodeTiket = "C-001", NamaTiket = "Coldplay VIP", Harga = 5_000_000, EventDate = new DateTime(2026, 6, 1), Quota = 100, CategoryId = concert.Id },
                new Ticket { KodeTiket = "C-002", NamaTiket = "Coldplay Festival", Harga = 2_500_000, EventDate = new DateTime(2026, 6, 1), Quota = 500, CategoryId = concert.Id },
                new Ticket { KodeTiket = "C-003", NamaTiket = "Bruno Mars Jakarta", Harga = 3_500_000, EventDate = new DateTime(2026, 9, 15), Quota = 250, CategoryId = concert.Id },
                new Ticket { KodeTiket = "C-004", NamaTiket = "Jazz Night", Harga = 750_000, EventDate = new DateTime(2026, 4, 20), Quota = 150, CategoryId = concert.Id },

                // SPORTS
                new Ticket { KodeTiket = "S-001", NamaTiket = "Football Regular", Harga = 250_000, EventDate = new DateTime(2026, 7, 10), Quota = 1000, CategoryId = sport.Id },
                new Ticket { KodeTiket = "S-002", NamaTiket = "Football VVIP", Harga = 1_500_000, EventDate = new DateTime(2026, 7, 10), Quota = 50, CategoryId = sport.Id },
                new Ticket { KodeTiket = "S-003", NamaTiket = "Badminton Masters", Harga = 450_000, EventDate = new DateTime(2026, 5, 5), Quota = 300, CategoryId = sport.Id },
                new Ticket { KodeTiket = "S-004", NamaTiket = "Basketball Finals", Harga = 800_000, EventDate = new DateTime(2026, 8, 20), Quota = 200, CategoryId = sport.Id },

                // FESTIVALS
                new Ticket { KodeTiket = "F-001", NamaTiket = "Summer Festival Day 1", Harga = 600_000, EventDate = new DateTime(2026, 8, 1), Quota = 1500, CategoryId = festival.Id },
                new Ticket { KodeTiket = "F-002", NamaTiket = "Summer Festival Day 2", Harga = 600_000, EventDate = new DateTime(2026, 8, 2), Quota = 1500, CategoryId = festival.Id },
                new Ticket { KodeTiket = "F-003", NamaTiket = "Food Carnival", Harga = 100_000, EventDate = new DateTime(2026, 3, 15), Quota = 2000, CategoryId = festival.Id },

                // EXHIBITIONS
                new Ticket { KodeTiket = "E-001", NamaTiket = "Modern Art Expo", Harga = 150_000, EventDate = new DateTime(2026, 4, 1), Quota = 300, CategoryId = exhibition.Id },
                new Ticket { KodeTiket = "E-002", NamaTiket = "Tech Conference", Harga = 1_200_000, EventDate = new DateTime(2026, 11, 10), Quota = 100, CategoryId = exhibition.Id }
            };

            context.Tickets.AddRange(tickets);
            await context.SaveChangesAsync();

            // ===== BOOKED TICKET (Initial Seed Booking) =====
            var seedBooking = new BookedTicket
            {
                BookingDate = DateTime.UtcNow
            };

            context.BookedTickets.Add(seedBooking);
            await context.SaveChangesAsync();

            // ===== BOOKED TICKET DETAILS =====
            var details = new List<BookedTicketDetail>
            {
                new BookedTicketDetail { BookedTicketId = seedBooking.Id, TicketId = tickets.First(t => t.KodeTiket == "C-001").Id, Quantity = 2 },
                new BookedTicketDetail { BookedTicketId = seedBooking.Id, TicketId = tickets.First(t => t.KodeTiket == "S-001").Id, Quantity = 3 },
                new BookedTicketDetail { BookedTicketId = seedBooking.Id, TicketId = tickets.First(t => t.KodeTiket =="F-003").Id, Quantity = 5 }
            };

            context.BookedTicketDetails.AddRange(details);
            await context.SaveChangesAsync();
        }
    }
}