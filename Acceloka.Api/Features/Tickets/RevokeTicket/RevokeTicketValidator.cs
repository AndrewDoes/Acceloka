using Acceloka.Api.Features.Tickets.RevokeTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Acceloka.Api.Features.Tickets.RevokeTicket
{
    public class RevokeTicketValidator : AbstractValidator<RevokeTicketCommand>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<RevokeTicketValidator> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RevokeTicketValidator(AccelokaDbContext db, ILogger<RevokeTicketValidator> logger, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = contextAccessor;
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
                    string error = $"Tiket {cmd.TicketCode} tidak ditemukan pada pesanan {cmd.BookedTicketId}";
                    _logger.LogInformation(error);
                    context.AddFailure("TicketCode", error);
                    return;
                }

                if (cmd.Quantity > detail.Quantity)
                {
                    string error = $"Jumlah yang di-revoke ({cmd.Quantity}) melebihi jumlah tiket yang dipesan ({detail.Quantity})";
                    _logger.LogInformation(error);
                    context.AddFailure("Quantity", error);
                }

            });

            RuleFor(x => x).CustomAsync(async (cmd, context, ct) =>
            {
                var userIdClaim = _httpContextAccessor.HttpContext!.User.FindFirstValue("InternalUserId");
                var userId = int.Parse(userIdClaim!);

                var detail = await db.BookedTicketDetails
                    .Include(d => d.BookedTicket)
                    .FirstOrDefaultAsync(d => d.BookedTicketId == cmd.BookedTicketId
                                             && d.Ticket.KodeTiket == cmd.TicketCode
                                             && d.BookedTicket.UserId == userId, ct);

                if (detail == null)
                {
                    string error = "Booking or Ticket code not found.";
                    _logger.LogInformation(error);
                    context.AddFailure("TicketCode", error);
                    return;
                }

                if (cmd.Quantity > detail.Quantity)
                {
                    string error = $"Cannot revoke {cmd.Quantity}. You only have {detail.Quantity} booked.";
                    _logger.LogInformation(error);
                    context.AddFailure("Quantity", error);
                }
            });
        }
    }
}
