using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WarOfMinds.Common.DTO;
using WarOfMinds.Context;
using WarOfMinds.Repositories;
using WarOfMinds.Services;
using WarOfMinds.WebApi.SignalR;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//my injections!
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddServices();
builder.Services.AddSignalR();
//builder.Services.Configure<HubOptions>(options =>
//{
//    options.HandshakeTimeout = TimeSpan.FromMinutes(5);
//    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
//});
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
builder.Services.AddSingleton<IDictionary<string, GroupData>>(opts => new Dictionary<string, GroupData>());
builder.Services.AddDbContext<IContext, DataContext>(options =>
{
    options.UseSqlServer("name=ConnectionStrings:WarOfMindsDB");
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();
app.MapHub<TriviaHub>("/TriviaHub");
app.MapControllers();

app.Run();
