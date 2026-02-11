namespace Acceloka.Api.Features.Tickets.EditBookedTicket.Responses
{
    public class EditBookedTicketResponse
    {
        public string TicketCode { get; set; } = default!;
        public string TicketName { get; set; } = default!;
        public int Quantity { get; set; }
        public string CategoryName { get; set; } = default!;
    }
}
