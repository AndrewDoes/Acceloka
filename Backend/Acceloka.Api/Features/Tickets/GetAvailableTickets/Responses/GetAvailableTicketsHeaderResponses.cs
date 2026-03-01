namespace Acceloka.Api.Features.Tickets.GetAvailableTickets.Responses
{
    public class GetAvailableTicketsHeaderResponses
    {
        public int totalTickets { get; set; }
        public List<GetAvailableTicketsResponse> tickets { get; set; }
    }
}
