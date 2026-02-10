using Acceloka.Api.Features.Tickets.Commands.BookTicket.Requests;
using Acceloka.Api.Features.Tickets.Queries.GetAvailableTickets;
using Acceloka.Api.Features.Tickets.Queries.GetBookedTicketDetail;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acceloka.Api.Controllers
{
    [ApiController]
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

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("get-booked-ticket/{bookedTicketId:int}")]
        public async Task<IActionResult> GetBookedTicketDetail(int bookedTicketId)
        {
            var result = await _mediator.Send(new GetBookedTicketDetailQuery(bookedTicketId));

            return Ok(result);
        }
    }
}
