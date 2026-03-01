using Acceloka.Api.Features.Tickets.GetAllBookings.Responses;
using MediatR;

namespace Acceloka.Api.Features.Tickets.GetAllBookings.Requests
{
    public record GetAllBookingsQuery() : IRequest<List<GetAllBookingsResponse>>;
}
