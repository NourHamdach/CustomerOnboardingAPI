using CustomerOnboarding.Api.Data;
using CustomerOnboarding.Api.Repositories;
using CustomerOnboarding.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Customer Onboarding API",
        Version = "v1",
        Description = @"API for customer registration and migration with OTP verification.

**Important Notes:**
- **VerificationType**: Use 'EMAIL' or 'MOBILE' (case-sensitive)
- **Phone Code**: Must start with + followed by 1-3 digits (e.g., +1, +44, +60, +971)
- **Phone Number**: 4-15 digits only, no spaces or special characters
- **IC Number**: Exactly 12 digits
- **PIN**: Exactly 6 digits (numbers only)
- **OTP Code**: Exactly 4 digits

**Testing Notes:**
- OTP codes are returned in the response (for testing only - remove in production)
- All phone numbers should be split into PhoneCode and PhoneNumber"
    });

    // Include XML comments for better API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOTPRepository, OTPRepository>();
builder.Services.AddScoped<IUserSecurityRepository, UserSecurityRepository>();

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Add exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
        Console.WriteLine($"INNER EXCEPTION: {ex.InnerException?.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message, stackTrace = ex.StackTrace });
    }
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
