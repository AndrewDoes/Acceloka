using Acceloka.Api.Domains.Entities;

namespace Acceloka.Api.Domain.Entities
{
    public class BookedTicket
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<BookedTicketDetail> BookedTicketDetails { get; set; }
            = new List<BookedTicketDetail>();
    }
}
