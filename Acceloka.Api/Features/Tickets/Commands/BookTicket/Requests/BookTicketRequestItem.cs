namespace Acceloka.Api.Features.Tickets.Commands.BookTicket.Requests
{
    public class BookTicketRequestItem
    {
        public string TicketCode { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
