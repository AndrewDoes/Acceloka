namespace Acceloka.Api.Features.Tickets.BookTicket.Responses
{
    public class BookTicketResponse
    {
        public decimal PriceSummary { get; set; }
        public List<TicketsPerCategoryResponse> TicketsPerCategories { get; set; } = new();
    }
}
