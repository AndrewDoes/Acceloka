namespace Acceloka.Api.Features.Tickets.Queries.GetBookedTicketDetail.Responses
{
    public class GetBookedTicketDetailResponse
    {
        public int QtyPerCategory { get; set; }
        public string CategoryName { get; set; } = default!;
        public List<BookedTicketDetailItemResponse> Tickets { get; set; } = [];
    }
}
