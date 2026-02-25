using Acceloka.Api.Features.Tickets.GetBookedTicketDetail.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.GetBookedTicketDetail
{
    public class GetBookedTicketDetailValidator : AbstractValidator<GetBookedTicketDetailQuery>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetBookedTicketDetailValidator(AccelokaDbContext db, ILogger logger , IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId must be provided.")
                .GreaterThan(0).WithMessage("BookedTicketId must be greater than 0.");

            RuleFor(x => x.BookedTicketId).CustomAsync(async (id, context, ct) =>
            {
                var exists = await _db.BookedTickets.AnyAsync(x => x.Id == id, ct);

                if (!exists)
                {
                    var error = $"BookedTicketId {id} not found";
                    _logger.LogInformation(error);
                    context.AddFailure("BookedTicketId", error);
                }
            });

            RuleFor(x => x).CustomAsync(async (query, context, ct) =>
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("InternalUserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    var error = "Session missing.";
                    context.AddFailure("User", error);
                    return;
                }

                var internalUserId = int.Parse(userIdClaim);
                var exists = await _db.BookedTickets.AnyAsync(x => x.Id == query.BookedTicketId && x.UserId == internalUserId, ct);

                if (!exists)
                {
                    string error = $"BookedTicketId {query.BookedTicketId} not found or access denied.";
                    _logger.LogInformation(error);
                    context.AddFailure("BookedTicketId", error);
                }
            });
        }
    }
}