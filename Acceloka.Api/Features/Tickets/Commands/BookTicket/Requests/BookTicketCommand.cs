using Acceloka.Api.Features.Tickets.Commands.BookTicket.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.Commands.BookTicket.Requests
{
    public class BookTicketCommand : IRequest<BookTicketResponse>
    {
        public List<BookTicketRequestItem> Tickets { get; set; } = new();
    }
}
