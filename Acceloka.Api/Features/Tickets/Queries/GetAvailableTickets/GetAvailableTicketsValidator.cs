using FluentValidation;

namespace Acceloka.Api.Features.Tickets.Queries.GetAvailableTickets
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsQuery>
    {
        public GetAvailableTicketsValidator()
        {
            //page validation
            RuleFor(x => x.page).GreaterThan(0);

            //page size validation
            RuleFor(x => x.pageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(50);
        }
    }
}
