using Microsoft.EntityFrameworkCore;
using WarOfMinds.Context;
using WarOfMinds.Repositories;
using WarOfMinds.Services;
using WarOfMinds.WebApi.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//my injections!
builder.Services.AddServices();
builder.Services.AddDbContext<IContext, DataContext>(options => options.UseSqlServer("name=ConnectionStrings:WarOfMindsDB"));
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHub<TriviaHub>("/TriviaHub");
app.MapControllers();

app.Run();
