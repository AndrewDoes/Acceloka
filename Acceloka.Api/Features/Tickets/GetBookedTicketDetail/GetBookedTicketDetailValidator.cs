using FluentValidation;

namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail
{
    public class GetBookedTicketDetailValidator : AbstractValidator<GetBookedTicketDetailQuery>
    {
        public GetBookedTicketDetailValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId must be provided.")
                .GreaterThan(0).WithMessage("BookedTicketId must be greater than 0.");
        }
    }
}