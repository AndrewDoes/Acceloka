using Acceloka.Api.Domains.Entities;
using Acceloka.Api.Features.Tickets.RevokeTicket.Requests;
using Acceloka.Api.Features.Tickets.RevokeTicket.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Acceloka.Api.Features.Tickets.RevokeTicket
{
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketCommand, RevokeTicketListResponse>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<RevokeTicketHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RevokeTicketHandler(AccelokaDbContext db, ILogger<RevokeTicketHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._db = db;
            this._logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RevokeTicketListResponse> Handle(RevokeTicketCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext!.User.FindFirstValue("InternalUserId");
            var userId = int.Parse(userIdClaim!);

            var detail = await _db.BookedTicketDetails
                .Include(d => d.BookedTicket)
                .Include(d => d.Ticket)
                    .ThenInclude(t => t.Category)
                .FirstAsync(d => d.BookedTicketId == request.BookedTicketId
                                 && d.Ticket.KodeTiket == request.TicketCode
                                 && d.BookedTicket.UserId == userId, cancellationToken);

            if (detail.Quantity <= request.Quantity)
            {
                _db.BookedTicketDetails.Remove(detail);
            }
            else
            {
                detail.Quantity -= request.Quantity;
            }

            await _db.SaveChangesAsync(cancellationToken);

            var remainingTickets = await _db.BookedTicketDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId)
                .Select(d => new RevokeTicketResponseItem
                {
                    TicketCode = d.Ticket.KodeTiket,
                    TicketName = d.Ticket.NamaTiket,
                    CategoryName = d.Ticket.Category.Name,
                    Quantity = d.Quantity
                }).ToListAsync(cancellationToken);

            if (!remainingTickets.Any())
            {
                _db.BookedTickets.Remove(detail.BookedTicket);
                await _db.SaveChangesAsync(cancellationToken);

                return new RevokeTicketListResponse
                {
                    Message = $"The entire booking {request.BookedTicketId} has been fully revoked and removed.",
                    RemainingTickets = new List<RevokeTicketResponseItem>()
                };
            }

            return new RevokeTicketListResponse
            {
                Message = "Ticket revoked successfully.",
                RemainingTickets = remainingTickets
            };
        }
    }
}