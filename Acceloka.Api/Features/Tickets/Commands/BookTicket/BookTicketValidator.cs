using Acceloka.Api.Features.Tickets.Commands.BookTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.Commands.BookTicket
{
    public class BookTicketValidator : AbstractValidator<BookTicketCommand>
    {
        private readonly AccelokaDbContext _db;

        public BookTicketValidator(AccelokaDbContext db)
        {
            _db = db;

            RuleFor(x => x.Tickets)
                .NotEmpty().WithMessage("At least one ticket must be booked.");

            RuleForEach(x => x.Tickets).CustomAsync(async (req, context, cancellationToken) =>
            {
                var bookingDate = DateTime.UtcNow;

                // 1. Check if Ticket exists [cite: 51]
                var ticket = await _db.Tickets
                    .Include(t => t.BookedTicketDetails)
                    .FirstOrDefaultAsync(t => t.KodeTiket == req.TicketCode, cancellationToken);

                if (ticket == null)
                {
                    context.AddFailure("TicketCode", $"Kode tiket {req.TicketCode} tidak terdaftar");
                    return;
                }

                var bookedQty = ticket.BookedTicketDetails.Sum(x => x.Quantity);
                var remainingQuota = ticket.Quota - bookedQty;

                if (remainingQuota <= 0)
                {
                    context.AddFailure("TicketCode", $"Quota tiket {ticket.KodeTiket} habis");
                }
                else if (req.Quantity > remainingQuota)
                {
                    context.AddFailure("Quantity", $"Quantity melebihi sisa quota untuk {ticket.KodeTiket}");
                }

                if (ticket.EventDate <= bookingDate)
                {
                    context.AddFailure("TicketCode", $"Tanggal event tiket {ticket.KodeTiket} tidak valid");
                }
            });
        }
    }
}