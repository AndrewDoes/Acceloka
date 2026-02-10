using MediatR;

namespace Acceloka.Api.Features.Tickets.Queries.GetAvailableTickets
{
    public class GetAvailableTicketsQuery : IRequest<List<GetAvailableTicketsResponse>>
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
