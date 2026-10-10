using txtOnly.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureAPI(builder.Configuration);
builder.Services.AddInfrastructureSwagger(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(options => options.Path = "/swagger");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");
app.Run();
