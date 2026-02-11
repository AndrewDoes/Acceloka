using Acceloka.Api.Features.Tickets.BookTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.BookTicket
{
    public class BookTicketValidator : AbstractValidator<BookTicketCommand>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<BookTicketValidator> _logger;

        public BookTicketValidator(AccelokaDbContext db, ILogger<BookTicketValidator> logger)
        {
            _db = db;
            _logger = logger;

            RuleFor(x => x.Tickets).Custom((tickets, context) =>
            {
                if (tickets == null || !tickets.Any())
                {
                    var error = "Harus memesan setidaknya satu tiket";
                    _logger.LogInformation(error); //
                    context.AddFailure(error);
                }
            });

            RuleFor(x => x.Tickets).Custom((tickets, context) =>
            {
                var duplicateCodes = tickets
                    .GroupBy(t => t.TicketCode)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateCodes.Any())
                {
                    var error = $"Duplicate ticket codes detected: {string.Join(", ", duplicateCodes)}";
                    _logger.LogInformation(error); 
                    context.AddFailure("Tickets", "Duplicate ticket codes are not allowed in a single booking.");
                }
            });

            RuleForEach(x => x.Tickets).Custom((ticket, context) =>
            {
                if (ticket.Quantity <= 0)
                {
                    var error = $"Invalid quantity ({ticket.Quantity}) for ticket {ticket.TicketCode}";
                    _logger.LogInformation(error);
                    context.AddFailure("Quantity", "Harus memesan 1 tiket atau lebih");
                }
            });

            RuleForEach(x => x.Tickets).CustomAsync(async (req, context, cancellationToken) =>
            {
                var bookingDate = DateTime.UtcNow;

                var ticket = await _db.Tickets
                    .Include(t => t.BookedTicketDetails)
                    .FirstOrDefaultAsync(t => t.KodeTiket == req.TicketCode, cancellationToken);

                if (ticket == null)
                {
                    var error = $"Kode tiket {req.TicketCode} tidak terdaftar";
                    _logger.LogInformation(error);
                    context.AddFailure("TicketCode", error);
                    return;
                }

                var bookedQty = ticket.BookedTicketDetails.Sum(x => x.Quantity);
                var remainingQuota = ticket.Quota - bookedQty;

                if (remainingQuota <= 0)
                {
                    var error = $"Quota tiket {ticket.KodeTiket} habis";
                    _logger.LogInformation(error);
                    context.AddFailure("TicketCode", error);
                }
                else if (req.Quantity > remainingQuota)
                {
                    var error = $"Quantity melebihi sisa quota untuk {ticket.KodeTiket}";
                    _logger.LogInformation(error);
                    context.AddFailure("Quantity", error);
                }

                if (ticket.EventDate <= bookingDate)
                {
                    var error = $"Tanggal event tiket {ticket.KodeTiket} tidak valid";
                    _logger.LogInformation(error);
                    context.AddFailure("TicketCode", error);
                }
            });
        }
    }
}