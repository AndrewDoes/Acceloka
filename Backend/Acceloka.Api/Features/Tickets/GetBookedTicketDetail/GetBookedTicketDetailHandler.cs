using Acceloka.Api.Domain.Entities;
using Acceloka.Api.Domains.Entities;
using Acceloka.Api.Features.Tickets.BookTicket.Responses;
using Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Requests;
using Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail
{
    public class GetBookedTicketDetailHandler : IRequestHandler<GetBookedTicketDetailQuery, List<GetBookedTicketDetailResponse>>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<GetBookedTicketDetailHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetBookedTicketDetailHandler(AccelokaDbContext db, ILogger<GetBookedTicketDetailHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetBookedTicketDetailResponse>> Handle(GetBookedTicketDetailQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext!.User.FindFirstValue("InternalUserId");
            var userId = int.Parse(userIdClaim!);

            var bookingDate = DateTime.UtcNow;

            var details = await _db.BookedTicketDetails
                .Include(d => d.Ticket)
                    .ThenInclude(t => t.Category)
                .Where(d => d.BookedTicketId == request.BookedTicketId && d.BookedTicket.UserId == userId)
                .ToListAsync(cancellationToken);

            // 3. Group the retrieved records by Category Name as per the response DTO
            return details
                .GroupBy(d => d.Ticket.Category.Name)
                .Select(g => new GetBookedTicketDetailResponse
                {
                    CategoryName = g.Key,
                    QtyPerCategory = g.Sum(x => x.Quantity),
                    Tickets = g.Select(x => new BookedTicketDetailItemResponse
                    {
                        TicketCode = x.Ticket.KodeTiket,
                        TicketName = x.Ticket.NamaTiket,
                        Quantity = x.Quantity,
                        EventDate = x.Ticket.EventDate.ToString("dd/MM/yyyy HH:mm:ss")
                    }).ToList()
                })
                .ToList();
        }
    }
}
