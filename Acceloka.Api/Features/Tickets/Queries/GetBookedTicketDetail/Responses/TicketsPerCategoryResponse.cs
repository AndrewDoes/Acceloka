namespace Acceloka.Api.Features.Tickets.Queries.GetBookedTicketDetail.Responses
{
    public class TicketsPerCategoryResponse
    {
        public string CategoryName { get; set; } = default!;
        public int QtyPerCategory { get; set; }
        public List<BookedTicketDetailItemResponse> Tickets { get; set; } = [];
    }
}
