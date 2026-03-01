using Acceloka.Api.Features.Tickets.GetAllBookings.Requests;
using FluentValidation;

namespace Acceloka.Api.Features.Tickets.GetAllBookings
{
    public class GetAllBookingsValidator : AbstractValidator<GetAllBookingsQuery>
    {
        private readonly ILogger<GetAllBookingsValidator> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllBookingsValidator(ILogger<GetAllBookingsValidator> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;


            RuleFor(x => x).Custom((query, context) =>
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("InternalUserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    var error = "Session missing. Please log in to view your bookings.";
                    _logger.LogInformation(error);
                    context.AddFailure("User", error);
                }
            });
        }
    }
}