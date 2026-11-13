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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();       //swagger/v1/swagger.json
    app.UseSwaggerUI();     //swagger/index.html
}

app.UseCors(options => options.WithOrigins("http://127.0.0.1:5500")
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var response = await PublicApiClient.SyncFromFormFav();

app.Run();
