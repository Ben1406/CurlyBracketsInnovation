using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using webApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(Globals.dbConnectionString));

var app = builder.Build();

Log.Write("WebApi starter");

app.UsePathBase(new PathString("/api"));
app.MapHealthChecks(new PathString("/health"));

app.UseHttpsRedirection();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
}

if (app.Environment.IsProduction())
{
    app.UseCors(options => options
    .WithOrigins("https://lcvinderup.dk")
    .AllowAnyHeader()
    .AllowAnyMethod());
}

app.UseAuthorization();
app.MapControllers();

var response = await PublicApiClient.SyncAllAddressFor7830();

Log.Write("WebApi startet");

app.Run();
