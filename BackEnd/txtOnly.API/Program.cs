using txtOnly.API.Middleware;
using txtOnly.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services.AddInfrastructureAPI(builder.Configuration);
builder.Services.AddInfrastructureSwagger(builder.Configuration);
builder.Services.AddInfrastructureCors(builder.Configuration);
builder.Services.AddInfrastructureRateLimiter(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.Path = "";
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();


app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
