using MassTransit;


var builder = WebApplication.CreateBuilder(args);

// 🔹 SignalR
builder.Services.AddSignalR();
builder.Services.AddControllers();

// 🔹 MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BidPlacedConsumer>();
    x.AddConsumer<AuctionEndedConsumer>();

    x.SetEndpointNameFormatter(
        new KebabCaseEndpointNameFormatter("notification", false)
    );

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

app.UseRouting();

app.MapHub<NotificationHub>("/hubs/notifications");
app.MapControllers();

app.Run();
