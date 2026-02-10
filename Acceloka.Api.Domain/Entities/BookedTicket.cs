namespace Acceloka.Api.Domain.Entities
{
    public class BookedTicket
    {
        public int Id { get; set; }

        public DateTime BookingDate { get; set; }

        public ICollection<BookedTicketDetail> BookedTicketDetails { get; set; }
            = new List<BookedTicketDetail>();
    }
}
