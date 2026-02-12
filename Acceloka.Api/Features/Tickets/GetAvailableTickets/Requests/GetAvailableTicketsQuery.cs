using Acceloka.Api.Features.Tickets.GetAvailableTickets.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.GetAvailableTickets.Requests
{
    public class GetAvailableTicketsQuery : IRequest<List<GetAvailableTicketsResponse>>
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;

        //sorting
        public string? OrderBy { get; set; }
        public string? OrderState { get; set; }

        //searching
        public string? CategoryName { get; set; }
        public string? TicketCode { get; set; }
        public string? TicketName { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartEventDate { get; set; }
        public DateTime? EndEventDate { get; set; }
    }
}
