using Acceloka.Api.Features.Tickets.RevokeTicket.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.RevokeTicket.Requests
{
    public record RevokeTicketCommand(int BookedTicketId, string TicketCode, int Quantity)
        : IRequest<List<RevokeTicketResponse>>;
}
