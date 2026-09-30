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

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<JerseyStore.Api.Services.IProductService, JerseyStore.Api.Services.InMemoryProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCors(FrontendCorsPolicy);
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthorization();

app.MapControllers();

app.Run();