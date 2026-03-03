using Acceloka.Api.Features.Tickets.AddTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.AddTicket
{
    public class AddTicketValidator : AbstractValidator<AddTicketCommand>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<AddTicketValidator> _logger;

        public AddTicketValidator(AccelokaDbContext db, ILogger<AddTicketValidator> logger)
        {
            _db = db;
            _logger = logger;

            RuleFor(x => x.TicketName)
                .NotEmpty()
                .WithMessage("Ticket name is required.");

            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .WithMessage("Category is required.");

            RuleFor(x => x.EventDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("Event date must be in the future.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Quota)
                .GreaterThan(0)
                .WithMessage("Quota must be at least 1.");
        }
    }
}