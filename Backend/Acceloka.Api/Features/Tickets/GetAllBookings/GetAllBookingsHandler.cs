using Acceloka.Api.Features.Tickets.GetAllBookings.Requests;
using Acceloka.Api.Features.Tickets.GetAllBookings.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Acceloka.Api.Features.Tickets.GetAllBookings
{
    public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsQuery, List<GetAllBookingsResponse>>
    {
        private readonly AccelokaDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllBookingsHandler(AccelokaDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetAllBookingsResponse>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("InternalUserId");
            if (string.IsNullOrEmpty(userIdClaim)) return new List<GetAllBookingsResponse>();

            var userId = int.Parse(userIdClaim);

            var bookings = await _db.BookedTickets
                .Where(b => b.UserId == userId)
                .Include(b => b.BookedTicketDetails)
                    .ThenInclude(d => d.Ticket)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);

            return bookings.Select(b => new GetAllBookingsResponse
            {
                BookingId = b.Id,
                BookingDate = b.BookingDate,
                TotalTickets = b.BookedTicketDetails.Sum(d => d.Quantity),
                TotalPrice = b.BookedTicketDetails.Sum(d => d.Quantity * d.Ticket.Harga)
            }).ToList();
        }
    }
}