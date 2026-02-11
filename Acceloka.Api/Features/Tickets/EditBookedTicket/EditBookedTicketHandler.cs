using Acceloka.Api.Features.Tickets.EditBookedTicket.Requests;
using Acceloka.Api.Features.Tickets.EditBookedTicket.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.EditBookedTicket
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketCommand, List<EditBookedTicketResponse>>
    {
        private readonly AccelokaDbContext _db;

        public EditBookedTicketHandler(AccelokaDbContext db)
        {
            this._db = db;
        }

        public async Task<List<EditBookedTicketResponse>> Handle(EditBookedTicketCommand request, CancellationToken cancellationToken)
        {
            var details = await _db.BookedTicketDetails
                .Include(t => t.Ticket)
                .ThenInclude(c => c.Category)
                .Where(d => d.BookedTicketId == request.BookedTicketId)
                .ToListAsync(cancellationToken);

            foreach (var req in request.Tickets)
            {
                var target = details.FirstOrDefault(d => d.Ticket.KodeTiket == req.TicketCode);
                target.Quantity = req.Quantity;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return details.Select(d => new EditBookedTicketResponse
            {
                TicketCode = d.Ticket.KodeTiket,
                TicketName = d.Ticket.NamaTiket,
                CategoryName = d.Ticket.Category.Name,
                Quantity = d.Quantity
            }).ToList();
        }
    }
}
