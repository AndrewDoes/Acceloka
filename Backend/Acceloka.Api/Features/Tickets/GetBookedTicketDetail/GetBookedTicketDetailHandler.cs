using Acceloka.Api.Domains.Entities;
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
        private readonly AccelokaDbContext _dbContext;
        private readonly ILogger<GetBookedTicketDetailHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetBookedTicketDetailHandler(AccelokaDbContext dbContext, ILogger<GetBookedTicketDetailHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetBookedTicketDetailResponse>> Handle(GetBookedTicketDetailQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext!.User.FindFirstValue("InternalUserId");
            var userId = int.Parse(userIdClaim!);


            var details = await _dbContext.BookedTicketDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId && d.BookedTicket.UserId == userId)
                .Include(d => d.Ticket)
                    .ThenInclude(t => t.Category)
                .ToListAsync(cancellationToken);

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
                        EventDate = x.Ticket.EventDate.ToString("dd/MM/yyyy hh:mm:ss")
                    }).ToList()
                })
                .ToList();
        }
    }
}
