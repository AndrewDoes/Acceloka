using Acceloka.Api.Features.Tickets.GetAvailableTickets.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.GetAvailableTickets.Requests
{
    public class GetAvailableTicketsQuery : IRequest<List<GetAvailableTicketsResponse>>
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
