using Acceloka.Api.Features.Tickets.EditBookedTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Acceloka.Api.Features.Tickets.Commands.EditBookedTicket
{
    public class EditBookedTicketValidator : AbstractValidator<EditBookedTicketCommand>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<EditBookedTicketValidator> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditBookedTicketValidator(AccelokaDbContext db, ILogger<EditBookedTicketValidator> logger, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            RuleFor(x => x.BookedTicketId).CustomAsync(async (id, context, ct) =>
            {
                var exists = await _db.BookedTickets.AnyAsync(x => x.Id == id, ct);
                if (!exists)
                {
                    context.AddFailure("BookedTicketId", "Booked tiketId tidak terdaftar");
                }
            });

            RuleForEach(x => x.Tickets).CustomAsync(async (req, context, ct) =>
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("InternalUserId");
                if (string.IsNullOrEmpty(userIdClaim)) return;
                var userId = int.Parse(userIdClaim);

                var cmd = context.InstanceToValidate;

                if (req.Quantity < 1)
                {
                    context.AddFailure("Quantity", "Quantity minimal 1.");
                    return;
                }

                var detail = await _db.BookedTicketDetails
                    .Include(d => d.Ticket)
                    .FirstOrDefaultAsync(d => d.BookedTicketId == cmd.BookedTicketId &&
                                              d.Ticket.KodeTiket == req.TicketCode &&
                                              d.BookedTicket.UserId == userId, ct);

                if (detail == null)
                {
                    context.AddFailure("TicketCode", $"Kode tiket {req.TicketCode} tidak terdaftar pada Bookedtiket {cmd.BookedTicketId}");
                    return;
                }

                var otherBookings = await _db.BookedTicketDetails
                    .Where(d => d.TicketId == detail.TicketId && d.Id != detail.Id)
                    .SumAsync(d => (int?)d.Quantity, ct) ?? 0;

                var remainingQuota = detail.Ticket.Quota - otherBookings;

                if (req.Quantity > remainingQuota)
                {
                    context.AddFailure("Quantity", $"Quantity {req.Quantity} melebihi total sisa quota ({remainingQuota})");
                }
            });
        }
    }
}