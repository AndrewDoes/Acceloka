using Acceloka.Api.Features.Tickets.EditBookedTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Api.Features.Tickets.Commands.EditBookedTicket
{
    public class EditBookedTicketValidator : AbstractValidator<EditBookedTicketCommand>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<EditBookedTicketValidator> _logger;

        public EditBookedTicketValidator(AccelokaDbContext db, ILogger<EditBookedTicketValidator> logger)
        {
            _db = db;
            _logger = logger;

            RuleFor(x => x.BookedTicketId).CustomAsync(async (id, context, ct) => {
                var exists = await _db.BookedTickets.AnyAsync(x => x.Id == id, ct);
                if (!exists)
                {
                    var error = $"Booked tiketId tidak terdaftar";
                    _logger.LogInformation(error);
                    context.AddFailure("BookedTicketId", error);
                }
            });

            RuleForEach(x => x.Tickets).Custom((ticket, context) =>
            {
                if (ticket.Quantity < 1)
                {
                    var error = $"Edit quantity failed: Ticket {ticket.TicketCode} requested quantity is {ticket.Quantity}, but minimal is 1.";
                    _logger.LogInformation(error);
                    context.AddFailure("Quantity", "Quantity minimal 1.");
                }
            });

            RuleForEach(x => x.Tickets).CustomAsync(async (req, context, ct) => {
                var cmd = context.InstanceToValidate;

                var detail = await _db.BookedTicketDetails
                    .Include(d => d.Ticket)
                    .FirstOrDefaultAsync(d => d.BookedTicketId == cmd.BookedTicketId &&
                                             d.Ticket.KodeTiket == req.TicketCode, ct);

                if (detail == null)
                {
                    var error = $"Kode tiket {req.TicketCode} tidak terdaftar pada Bookedtiket {cmd.BookedTicketId}";
                    _logger.LogInformation(error);
                    context.AddFailure("TicketCode", error);
                    return;
                }

                var otherBookings = await _db.BookedTicketDetails
                    .Where(d => d.TicketId == detail.TicketId && d.Id != detail.Id)
                    .SumAsync(d => (int?)d.Quantity, ct) ?? 0;

                var remainingQuota = detail.Ticket.Quota - otherBookings;

                if (req.Quantity > remainingQuota)
                {
                    var error = $"Quantity {req.Quantity} melebihi total sisa quota ({remainingQuota})";
                    _logger.LogInformation(error);
                    context.AddFailure("Quantity", error);
                }
            });
        }
    }
}