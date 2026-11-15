using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using webApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddMvc();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Globals.dbConnectionString));

var app = builder.Build();

Log.Write("WebApi starter");

app.UsePathBase("/api");
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

var origins = new string[] {
    "http://127.0.0.1:5500",
    "http://lcvinderup.dk",
    "https://lcvinderup.dk",
    "http://nt28.unoeuro.com",
    "https://nt28.unoeuro.com"
};

app.UseCors(options => options.WithOrigins(origins)
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

var response = await PublicApiClient.SyncAllAddressFor7830();

Log.Write("WebApi startet");

app.Run();
