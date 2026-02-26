namespace Acceloka.Api.Features.Tickets.GetAvailableTickets.Responses
{
    public class GetAvailableTicketsResponse
    {
        public string EventDate {  get; set; }
        public int Quota { get; set; }
        public string TicketCode { get; set; }
        public string TicketName { get; set; }
        public string Category {  get; set; }
        public decimal Price { get; set; }

    }
}
