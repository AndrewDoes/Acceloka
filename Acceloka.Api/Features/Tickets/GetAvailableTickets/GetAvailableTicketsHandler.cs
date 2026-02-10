using Acceloka.Api.Features.Tickets.GetAvailableTickets.Requests;
using Acceloka.Api.Features.Tickets.GetAvailableTickets.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.GetAvailableTickets
{
    public class GetAvailableTicketsHandler : IRequestHandler<GetAvailableTicketsQuery, List<GetAvailableTicketsResponse>>
    {
        private readonly AccelokaDbContext _dbContext;
        public GetAvailableTicketsHandler(AccelokaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public  async Task<List<GetAvailableTicketsResponse>> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
        {
            //checkig on pages
            var skip = (request.page - 1) * request.pageSize;

            var tickets = await _dbContext.Tickets
            .Include(t => t.Category)
            .Include(t => t.BookedTicketDetails)
            .Where(t =>
                t.Quota >
                (t.BookedTicketDetails.Sum(d => (int?)d.Quantity) ?? 0)
            )
            .OrderBy(t => t.NamaTiket)
            .Skip(skip)
            .Take(request.pageSize)
            .Select(t => new GetAvailableTicketsResponse
            {
                EventDate = t.EventDate.ToString("dd/MM/yyyy HH:mm:ss"),
                TicketCode = t.KodeTiket,
                TicketName = t.NamaTiket,
                Price = t.Harga,
                Category = t.Category.Name,
                Quota =
                    t.Quota -
                    (t.BookedTicketDetails.Sum(d => (int?)d.Quantity) ?? 0)
            })
            .ToListAsync(cancellationToken);

            return tickets;
        }
    }
}
