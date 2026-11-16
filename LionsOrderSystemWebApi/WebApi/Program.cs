using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using webApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Globals.dbConnectionString));

var app = builder.Build();

Log.Write("WebApi startet");

app.UsePathBase("/api");
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

var origins = new string[] {
    "http://127.0.0.1:5500",
    "https://lcvinderup.dk"
};

app.UseCors(options => options.WithOrigins(origins)
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseAuthorization();
app.MapControllers();

var response = await PublicApiClient.SyncAllAddressFor7830();

Log.Write("WebApi startet");

app.Run();
