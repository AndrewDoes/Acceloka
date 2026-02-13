using Acceloka.Api.Features.Tickets.RevokeTicket.Requests;
using Acceloka.Api.Features.Tickets.RevokeTicket.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.RevokeTicket
{
    // Fix: Return RevokeTicketListResponse, not List<RevokeTicketListResponse>
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketCommand, RevokeTicketListResponse>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<RevokeTicketHandler> _logger;

        public RevokeTicketHandler(AccelokaDbContext db, ILogger<RevokeTicketHandler> logger)
        {
            this._db = db;
            this._logger = logger;
        }

        public async Task<RevokeTicketListResponse> Handle(RevokeTicketCommand request, CancellationToken cancellationToken)
        {
            var detail = await _db.BookedTicketDetails
                .Include(d => d.Ticket)
                .FirstOrDefaultAsync(d => d.BookedTicketId == request.BookedTicketId && d.Ticket.KodeTiket == request.TicketCode, cancellationToken);

            if (detail == null)
            {
                throw new KeyNotFoundException($"Ticket with code {request.TicketCode} was not found in booking {request.BookedTicketId}.");
            }

            if (detail.Quantity <= request.Quantity)
            {
                _db.BookedTicketDetails.Remove(detail);
            }
            else
            {
                detail.Quantity -= request.Quantity;
            }

            var remainingDetailsCount = await _db.BookedTicketDetails
                .CountAsync(d => d.BookedTicketId == request.BookedTicketId && d.Id != detail.Id, cancellationToken);

            bool isBookingFullyDeleted = false;

            if (detail.Quantity <= request.Quantity && remainingDetailsCount == 0)
            {
                var parentHeader = await _db.BookedTickets
                    .FirstOrDefaultAsync(b => b.Id == request.BookedTicketId, cancellationToken);

                if (parentHeader != null)
                {
                    _db.BookedTickets.Remove(parentHeader);
                    isBookingFullyDeleted = true;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            if (isBookingFullyDeleted)
            {
                return new RevokeTicketListResponse
                {
                    Message = $"The entire booking {request.BookedTicketId} has been fully revoked and removed.",
                    RemainingTickets = new List<RevokeTicketResponseItem>()
                };
            }

            // Fix: Project to RevokeTicketResponseItem, not RevokeTicketListResponse
            var remainingTickets = await _db.BookedTicketDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId)
                .Select(d => new RevokeTicketResponseItem
                {
                    TicketCode = d.Ticket.KodeTiket,
                    TicketName = d.Ticket.NamaTiket,
                    CategoryName = d.Ticket.Category.Name,
                    Quantity = d.Quantity
                }).ToListAsync(cancellationToken);

            return new RevokeTicketListResponse
            {
                Message = "Ticket revoked successfully.",
                RemainingTickets = remainingTickets
            };
        }
    }
}