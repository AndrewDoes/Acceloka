using Acceloka.Api.Features.Tickets.BookTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.BookTicket
{
    public class BookTicketValidator : AbstractValidator<BookTicketCommand>
    {
        private readonly AccelokaDbContext _db;

        public BookTicketValidator(AccelokaDbContext db)
        {
            _db = db;

            RuleFor(x => x.Tickets)
                .NotEmpty().WithMessage("Harus memesan setidaknya satu tiket");

            RuleFor(x => x.Tickets)
            .Must(x => x.Select(t => t.TicketCode).Distinct().Count() == x.Count)
            .WithMessage("Duplicate ticket codes are not allowed in a single booking.");

            RuleForEach(x => x.Tickets).ChildRules(ticket => {
                ticket.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage($"Harus memesan 1 tiket atau lebih");
            });

            RuleForEach(x => x.Tickets).CustomAsync(async (req, context, cancellationToken) =>
            {
                var bookingDate = DateTime.UtcNow;

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