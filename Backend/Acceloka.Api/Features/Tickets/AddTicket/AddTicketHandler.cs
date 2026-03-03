using Acceloka.Api.Domain.Entities;
using Acceloka.Api.Domains.Entities;
using Acceloka.Api.Features.Tickets.AddTicket.Requests;
using Acceloka.Api.Features.Tickets.AddTicket.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.AddTicket
{
    public class AddTicketHandler : IRequestHandler<AddTicketCommand, AddTicketResponse>
    {
        private readonly AccelokaDbContext _db;

        public AddTicketHandler(AccelokaDbContext db)
        {
            _db = db;
        }

        public async Task<AddTicketResponse> Handle(AddTicketCommand request, CancellationToken cancellationToken)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name == request.CategoryName, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category '{request.CategoryName}' not found.");
            }

            char prefix = char.ToUpper(category.Name.Trim()[0]);
            string prefixMatch = $"{prefix}-";

            int categoryCount = await _db.Tickets
                .CountAsync(t => t.CategoryId == category.Id, cancellationToken);

            var latestTicket = await _db.Tickets
                .Where(t => t.KodeTiket.StartsWith(prefixMatch))
                .OrderByDescending(t => t.KodeTiket)
                .FirstOrDefaultAsync(cancellationToken);

            int highestNumberFromCode = 0;
            if (latestTicket != null)
            {
                var parts = latestTicket.KodeTiket.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int currentMax))
                {
                    highestNumberFromCode = currentMax;
                }
            }

            int nextNumber = Math.Max(categoryCount, highestNumberFromCode) + 1;
            string autoTicketCode = $"{prefix}-{nextNumber.ToString("D3")}";

            var ticket = new Ticket
            {
                NamaTiket = request.TicketName,
                KodeTiket = autoTicketCode,
                CategoryId = category.Id,
                EventDate = request.EventDate,
                Harga = request.Price,
                Quota = request.Quota
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync(cancellationToken);

            return new AddTicketResponse
            {
                Message = "Ticket published successfully.",
                TicketId = ticket.Id,
                TicketCode = ticket.KodeTiket
            };
        }
    }
}