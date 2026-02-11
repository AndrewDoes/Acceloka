using Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail
{
    public class GetBookedTicketDetailValidator : AbstractValidator<GetBookedTicketDetailQuery>
    {
        private readonly AccelokaDbContext _db;

        public GetBookedTicketDetailValidator(AccelokaDbContext db)
        {
            _db = db;

            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId must be provided.")
                .GreaterThan(0).WithMessage("BookedTicketId must be greater than 0.");

            RuleFor(x => x.BookedTicketId).CustomAsync(async (id, context, ct) =>
            {
                var exists = await _db.BookedTickets.AnyAsync(x => x.Id == id, ct);

                if (!exists)
                {
                    context.AddFailure("BookedTicketId", $"BookedTicketId {id} not found");
                }
            });
        }
    }
}