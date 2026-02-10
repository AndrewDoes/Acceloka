namespace Acceloka.Api.Features.Tickets.Queries.GetBookedTicketDetail.Responses
{
    public class BookedTicketDetailItemResponse
    {
        public string TicketCode { get; set; } = default!;
        public string TicketName { get; set; } = default!;
        public string EventDate { get; set; }
    }
}
