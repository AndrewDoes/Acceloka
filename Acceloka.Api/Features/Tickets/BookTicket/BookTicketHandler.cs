using Acceloka.Api.Domain.Entities;
using Acceloka.Api.Features.Tickets.BookTicket.Requests;
using Acceloka.Api.Features.Tickets.BookTicket.Responses;
using Acceloka.Api.Features.Tickets.BookTicket.Responses;
using Acceloka.Api.Features.Tickets.BookTicket.Responses;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

public class BookTicketHandler
    : IRequestHandler<BookTicketCommand, BookTicketResponse>
{
    private readonly AccelokaDbContext _db;
    private readonly ILogger<BookTicketHandler> _logger;

    public BookTicketHandler(AccelokaDbContext db, ILogger<BookTicketHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<BookTicketResponse> Handle(BookTicketCommand request, CancellationToken cancellationToken)
    {
        var bookingDate = DateTime.UtcNow;

        //load ticket by ticket code
        var ticketCode = request.Tickets.Select(x => x.TicketCode).ToList();

        var tickets = await _db.Tickets
             .Include(t => t.Category)
             .Include(t => t.BookedTicketDetails)
             .Where(t => ticketCode.Contains(t.KodeTiket))
             .ToListAsync(cancellationToken);

        var bookedTicket = new BookedTicket
        {
            BookingDate = bookingDate,
            BookedTicketDetails = request.Tickets.Select(req =>
            {
                var ticket = tickets.First(t => t.KodeTiket == req.TicketCode);
                return new BookedTicketDetail
                {
                    TicketId = ticket.Id,
                    Quantity = req.Quantity
                };
            }).ToList()
        };

        _db.BookedTickets.Add(bookedTicket);
        await _db.SaveChangesAsync(cancellationToken);

        var categoryGroups = request.Tickets.Select(req =>
        {
            var ticket = tickets.First(t => t.KodeTiket == req.TicketCode);
            return new
            {
                CategoryName = ticket.Category.Name,
                TicketCode = ticket.KodeTiket,
                TicketName = ticket.NamaTiket,
                LinePrice = req.Quantity * ticket.Harga
            };
        })
            .GroupBy(x => x.CategoryName)
            .Select(g => new TicketsPerCategoryResponse
            {
                CategoryName = g.Key,
                SummaryPrice = g.Sum(x => x.LinePrice),
                Tickets = g.Select(x => new BookedTicketResponse
                {
                    TicketCode = x.TicketCode,
                    TicketName = x.TicketName,
                    Price = x.LinePrice
                }).ToList()
            })
            .ToList();

        return new BookTicketResponse
        {
            PriceSummary = categoryGroups.Sum(x => x.SummaryPrice),
            TicketsPerCategories = categoryGroups
        };

    }
}
