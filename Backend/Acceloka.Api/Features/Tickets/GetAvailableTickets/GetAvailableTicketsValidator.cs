using Acceloka.Api.Features.Tickets.GetAvailableTickets.Requests;
using FluentValidation;

namespace Acceloka.Api.Features.Tickets.GetAvailableTickets
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsQuery>
    {
        public GetAvailableTicketsValidator()
        {
            //page validation
            RuleFor(x => x.page).GreaterThan(0).WithMessage("Page must be more than 0");

            //page size validation
            RuleFor(x => x.pageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(50)
                .WithMessage("Pagesize cannot be less than 0 and more than 50");
        }
    }
}
