using Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail
{
    public class GetBookedTicketDetailHandler : IRequestHandler<GetBookedTicketDetailQuery, List<GetBookedTicketDetailResponse>>
    {
        private readonly AccelokaDbContext _dbContext;
        private readonly ILogger<GetBookedTicketDetailHandler> _logger;

        public GetBookedTicketDetailHandler(AccelokaDbContext dbContext, ILogger<GetBookedTicketDetailHandler> logger)
        {
            this._dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<GetBookedTicketDetailResponse>> Handle(GetBookedTicketDetailQuery request, CancellationToken cancellationToken)
        {
            // 1️⃣ Validate existence
            var exists = await _dbContext.BookedTickets
                .AnyAsync(x => x.Id == request.BookedTicketId, cancellationToken);

            if (!exists)
            {
                var error = $"BookedTicketId not found";
                _logger.LogInformation(error);
                throw new BadHttpRequestException(error, StatusCodes.Status400BadRequest);
            }

            // 2️⃣ Load data
            var details = await _dbContext.BookedTicketDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId)
                .Include(d => d.Ticket)
                    .ThenInclude(t => t.Category)
                .ToListAsync(cancellationToken);

            // 3️⃣ Group & map
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
