using Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Requests
{
    public record GetBookedTicketDetailQuery(int BookedTicketId)
    : IRequest<List<GetBookedTicketDetailResponse>>;
}
