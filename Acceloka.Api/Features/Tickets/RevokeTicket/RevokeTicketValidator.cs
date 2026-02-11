using Acceloka.Api.Features.Tickets.RevokeTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.RevokeTicket
{
    public class RevokeTicketValidator : AbstractValidator<RevokeTicketCommand>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<RevokeTicketValidator> _logger;
        public RevokeTicketValidator(AccelokaDbContext db, ILogger<RevokeTicketValidator> logger)
        {
            _db = db;
            _logger = logger;
            RuleFor(x => x.BookedTicketId).GreaterThan(0).WithMessage("BookedTicketId must be greater than 0.");
            RuleFor(x => x.TicketCode).NotEmpty().WithMessage("TicketCode must not be empty.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be higher than 0");

            RuleFor(x => x).CustomAsync(async (cmd, context, ct) =>
            {
                var detail = await _db.BookedTicketDetails
                    .Include(d => d.Ticket)
                    .FirstOrDefaultAsync(d => d.BookedTicketId == cmd.BookedTicketId &&
                                             d.Ticket.KodeTiket == cmd.TicketCode, ct);

                if (detail == null)
                {
                    context.AddFailure("TicketCode", $"Tiket {cmd.TicketCode} tidak ditemukan pada pesanan {cmd.BookedTicketId}");
                    return;
                }

                if (cmd.Quantity > detail.Quantity)
                {
                    context.AddFailure("Quantity", $"Jumlah yang di-revoke ({cmd.Quantity}) melebihi jumlah tiket yang dipesan ({detail.Quantity})");
                }

            });
        }
    }
}
