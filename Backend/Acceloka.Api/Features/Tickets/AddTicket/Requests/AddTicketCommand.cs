using Acceloka.Api.Features.Tickets.AddTicket.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.AddTicket.Requests
{
    public record AddTicketCommand(
        string TicketName,
        string CategoryName,
        DateTime EventDate,
        decimal Price,
        int Quota
    ) : IRequest<AddTicketResponse>;
}
