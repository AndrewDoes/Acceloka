using Acceloka.Api.Features.Tickets.Commands.BookTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using Acceloka.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args); 

// Configure Serilog 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: $"logs/Log-{DateTime.Now:yyyyMMdd}.txt")
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog as the logging provider

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//dbContext
builder.Services.AddDbContext<AccelokaDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//mediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BookTicketCommand).Assembly));

//RFC 7807 Standard
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        // This ensures only the essential RFC 7807 info is sent to Postman
        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "Bad Request",
            status = StatusCodes.Status400BadRequest,
            detail = exception?.Message, // This is your "Quota Habis" message
            traceId = context.TraceIdentifier
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});
app.UseStatusCodePages();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AccelokaDbContext>();
    await DbInitializer.SeedAsync(dbContext);
}

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            diagnosticContext.Set("Exception", exception.Message);
        }
    };
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
