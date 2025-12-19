using System.Text.Json.Serialization;
using Calls.Application.Interfaces;
using Calls.Application.Interfaces.External;
using Calls.Application.Services;
using Calls.Domain.Rooms;
using Calls.Infrastructure.ChangerNotifier;
using Calls.Infrastructure.Data;
using Calls.Infrastructure.Rooms.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=CallsDb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<CallsDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IRoomRepository, EfRoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ISignalingService, SignalingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CallsDbContext>();
        dbContext.Database.Migrate();
    }
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();