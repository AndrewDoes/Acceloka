using Acceloka.Api.Features.Tickets.EditBookedTicket.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace Acceloka.Api.Features.Tickets.EditBookedTicket.Requests
{
    public class EditBookedTicketCommand : IRequest<List<EditBookedTicketResponse>>
    {
        [JsonIgnore]
        public int BookedTicketId { get; set; }
        public List<EditBookedTicketRequestItem> Tickets { get; set; } = new();
    }
}
