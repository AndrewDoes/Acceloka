using Acceloka.Api.Common;
using Acceloka.Api.Domains.Entities;
using Acceloka.Api.Features.Tickets.AddTicket.Requests;
using Acceloka.Api.Features.Tickets.BookTicket.Requests;
using Acceloka.Api.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

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
string[] allowedOrigins = {
    "http://localhost:3000",
    "http://192.168.56.1:3000",
    //Add any port here to configure with the front end in the Next.js npm run dev
};


//mediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BookTicketCommand).Assembly));
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(AddTicketCommand).Assembly);
});

//FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//RFC 7807 Standard
builder.Services.AddProblemDetails();

//CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    options.Events.OnTicketReceived = async context =>
    {
        var db = context.HttpContext.RequestServices.GetRequiredService<AccelokaDbContext>();
        var googleId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = context.Principal.FindFirstValue(ClaimTypes.Email);

        var user = await db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

        if (user == null)
        {
            user = new User { GoogleId = googleId, Email = email };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var identity = context.Principal?.Identity as ClaimsIdentity;
        if (identity != null)
        {
            identity.AddClaim(new Claim("InternalUserId", user.Id.ToString()));
            var roleValue = !string.IsNullOrEmpty(user.Role) ? user.Role : "User";
            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
        }
    };
});

builder.Services.AddHttpContextAccessor();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5225);
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "Bad Request",
            status = StatusCodes.Status400BadRequest,
            detail = exception?.Message,
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

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
