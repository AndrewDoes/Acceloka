using Acceloka.Api.Features.Tickets.Queries.GetBookedTicketDetail.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.Queries.GetBookedTicketDetail
{
    public record GetBookedTicketDetailQuery(int BookedTicketId)
    : IRequest<List<GetBookedTicketDetailResponse>>;
}
