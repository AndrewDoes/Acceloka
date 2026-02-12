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
        public async Task<List<GetAvailableTicketsResponse>> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
        {
            //checkig on pages
            var skip = (request.page - 1) * request.pageSize;

            var query = _dbContext.Tickets
         .Include(t => t.Category)
         .Include(t => t.BookedTicketDetails)
         .AsQueryable();

            query.Where(t => t.Quota > (t.BookedTicketDetails.Sum(d => (int?)d.Quantity) ?? 0));

            //filtering
            query = HandleFilterRequest(request, query);
            //sorting
            query = HandleSortRequest(request, query);

            return await query
                .Skip(skip)
                .Take(request.pageSize)
                .Select(t => new GetAvailableTicketsResponse
                {
                    EventDate = t.EventDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    TicketCode = t.KodeTiket,
                    TicketName = t.NamaTiket,
                    Price = t.Harga,
                    Category = t.Category.Name,
                    Quota = t.Quota - (t.BookedTicketDetails.Sum(d => (int?)d.Quantity) ?? 0)
                })
                .ToListAsync(cancellationToken);
        }

        private static IQueryable<Domain.Entities.Ticket> HandleSortRequest(GetAvailableTicketsQuery request, IQueryable<Domain.Entities.Ticket> query)
        {
            var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "TicketCode" : request.OrderBy;
            var orderState = string.IsNullOrWhiteSpace(request.OrderState) ? "asc" : request.OrderState.ToLower();

            query = orderBy switch
            {
                "TicketName" => orderState == "desc" ? query.OrderByDescending(t => t.NamaTiket) : query.OrderBy(t => t.NamaTiket),
                "Category" => orderState == "desc" ? query.OrderByDescending(t => t.Category.Name) : query.OrderBy(t => t.Category.Name),
                "Price" => orderState == "desc" ? query.OrderByDescending(t => t.Harga) : query.OrderBy(t => t.Harga),
                _ => orderState == "desc" ? query.OrderByDescending(t => t.KodeTiket) : query.OrderBy(t => t.KodeTiket)
            };
            return query;
        }

        private static IQueryable<Domain.Entities.Ticket> HandleFilterRequest(GetAvailableTicketsQuery request, IQueryable<Domain.Entities.Ticket> query)
        {
            if (!string.IsNullOrWhiteSpace(request.CategoryName))
                query = query.Where(t => t.Category.Name.Contains(request.CategoryName));

            if (!string.IsNullOrWhiteSpace(request.TicketCode))
                query = query.Where(t => t.KodeTiket.Contains(request.TicketCode));

            if (!string.IsNullOrWhiteSpace(request.TicketName))
                query = query.Where(t => t.NamaTiket.Contains(request.TicketName));

            if (request.MaxPrice.HasValue)
                query = query.Where(t => t.Harga <= request.MaxPrice.Value);

            if (request.StartEventDate.HasValue)
                query = query.Where(t => t.EventDate >= request.StartEventDate.Value);

            if (request.EndEventDate.HasValue)
                query = query.Where(t => t.EventDate <= request.EndEventDate.Value);
            return query;
        }
    }
}
