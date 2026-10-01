using JerseyStore.Api.Data;
using JerseyStore.Api.Services;
using Microsoft.EntityFrameworkCore;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

var frontendOrigin = builder.Configuration["Cors:AllowedOrigin"]
    ?? throw new InvalidOperationException("Cors:AllowedOrigin must be configured.");

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IProductService, EfProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseRouting();
app.UseCors(FrontendCorsPolicy);
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;