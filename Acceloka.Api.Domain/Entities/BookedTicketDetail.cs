namespace Acceloka.Api.Domain.Entities
{
    public class BookedTicketDetail
    {
        public int Id { get; set; }
        public int BookedTicketId { get; set; } //buat reference ke BookedTicket biasa
        public BookedTicket BookedTicket { get; set; } = null!;
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
