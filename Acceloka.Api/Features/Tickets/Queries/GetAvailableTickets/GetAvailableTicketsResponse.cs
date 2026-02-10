namespace Acceloka.Api.Features.Tickets.Queries.GetAvailableTickets
{
    public class GetAvailableTicketsResponse
    {
        public int TicketId { get; set; }
        public string TicketName { get; set; }
        public decimal Price { get; set; }
        public int RemainingQuantity { get; set; }
        public string Category {  get; set; }
    }
}
