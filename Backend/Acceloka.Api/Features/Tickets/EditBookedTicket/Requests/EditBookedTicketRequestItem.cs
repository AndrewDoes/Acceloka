namespace Acceloka.Api.Features.Tickets.EditBookedTicket.Requests
{
    public class EditBookedTicketRequestItem
    {
        public string TicketCode { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
