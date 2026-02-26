using System.ComponentModel.DataAnnotations.Schema;

namespace Acceloka.Api.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string KodeTiket { get; set; }
        public string NamaTiket { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Harga { get; set; }
        public DateTime EventDate { get; set; }
        public int Quota { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<BookedTicketDetail> BookedTicketDetails { get; set; }
        = new List<BookedTicketDetail>();
    }
}
