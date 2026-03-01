namespace Acceloka.Api.Features.Tickets.GetAllBookings.Responses
{
    public class GetAllBookingsResponse
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalTickets { get; set; }
    }
}
