namespace Acceloka.Api.Features.Tickets.RevokeTicket.Responses
{
    public class RevokeTicketListResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<RevokeTicketResponseItem> RemainingTickets { get; set; } = new();
    }
}
