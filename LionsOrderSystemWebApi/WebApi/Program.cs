using Microsoft.EntityFrameworkCore;
using webApi;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddMvc();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UsePathBase("/api");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();       //swagger/v1/swagger.json
    app.UseSwaggerUI();     //swagger/index.html
}

var origins = new string[] {
    "http://lcvinderup.dk",
    "https://lcvinderup.dk",
    "http://127.0.0.1:5500",
    "http://nt28.unoeuro.com",
    "https://nt28.unoeuro.com"
};

app.UseCors(options => options.WithOrigins(origins)
    .AllowAnyHeader()
    .AllowAnyMethod());

app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var response = await PublicApiClient.SyncAllAddressFor7830();

var dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());

var log = new Log()
{
    tidspunkt = DateTime.Now,
    besked = "WebApi started"
};
dbContext.Log.Add(log);
dbContext.SaveChanges();

app.Run();
