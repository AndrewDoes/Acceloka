using Acceloka.Api.Features.Tickets.Queries.GetAvailableTickets;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acceloka.Api.Controllers
{
    [Route("api/v1")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("get-available-tickets")]
        public async Task<IActionResult> GetAvailabletickets([FromQuery] GetAvailableTicketsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
