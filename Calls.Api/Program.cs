using System.Text.Json.Serialization;
using Calls.Application.Interfaces;
using Calls.Application.Interfaces.External;
using Calls.Application.Services;
using Calls.Domain.Rooms;
using Calls.Infrastructure.ChangerNotifier;
using Calls.Infrastructure.Rooms.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IChangerNotifierClient, HttpChangerNotifierClient>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseAddress =
        configuration["ChangerNotifier__BaseAddress"] ??
        configuration["ChangerNotifier:BaseAddress"] ??
        "http://changer-notifier:5001";

    client.BaseAddress = new Uri(baseAddress);
});

builder.Services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ISignalingService, SignalingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();