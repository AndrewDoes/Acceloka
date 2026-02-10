namespace Acceloka.Api.Features.Tickets.Commands.BookTicket.Responses
{
    public class BookedTicketResponse
    {
        public string TicketCode { get; set; } = null!;
        public string TicketName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
