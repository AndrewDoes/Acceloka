namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Responses
{
    public class BookedTicketDetailItemResponse
    {
        public string TicketCode { get; set; } = default!;
        public string TicketName { get; set; } = default!;
        public int Quantity { get; set; }
        public string EventDate { get; set; }
    }
}
