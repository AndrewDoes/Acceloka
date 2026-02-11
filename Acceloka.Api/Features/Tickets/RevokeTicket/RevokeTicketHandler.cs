using Acceloka.Api.Features.Tickets.RevokeTicket.Requests;
using Acceloka.Api.Features.Tickets.RevokeTicket.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.RevokeTicket
{
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketCommand, List<RevokeTicketResponse>>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<RevokeTicketHandler> _logger;

        public RevokeTicketHandler(AccelokaDbContext db, ILogger<RevokeTicketHandler> logger)
        {
            this._db = db;
            this._logger = logger;
        }

        public async Task<List<RevokeTicketResponse>> Handle(RevokeTicketCommand request, CancellationToken cancellationToken)
        {
            var detail = await _db.BookedTicketDetails
                .FirstOrDefaultAsync(d => d.BookedTicketId == request.BookedTicketId && d.Ticket.KodeTiket == request.TicketCode, cancellationToken);

            if (detail.Quantity <= request.Quantity)
            {
                _db.BookedTicketDetails.Remove(detail);
            }
            else
            {
                detail.Quantity -= request.Quantity;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return await _db.BookedTicketDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId)
                .Select(d => new RevokeTicketResponse
                {
                    TicketCode = d.Ticket.KodeTiket,
                    TicketName = d.Ticket.NamaTiket,
                    CategoryName = d.Ticket.Category.Name,
                    Quantity = d.Quantity
                }).ToListAsync(cancellationToken);
        }
    }
}
