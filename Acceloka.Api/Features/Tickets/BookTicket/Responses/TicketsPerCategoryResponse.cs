namespace Acceloka.Api.Features.Tickets.BookTicket.Responses
{
    public class TicketsPerCategoryResponse
    {
        public string CategoryName { get; set; } = null!;
        public decimal SummaryPrice { get; set; }
        public List<BookedTicketResponse> Tickets { get; set; } = new();
    }
}
