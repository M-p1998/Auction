using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Consumers;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

// var rabbitSection = builder.Configuration.GetSection("RabbitMq");
// var rabbitHost = rabbitSection["Host"];
// var rabbitUser = rabbitSection["Username"];
// var rabbitPass = rabbitSection["Password"];
// builder.Services.AddMassTransit(x =>
// {
//     x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
//     x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

//     x.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.Host(rabbitHost, "/", h =>
//         {
//             h.Username(rabbitUser);
//             h.Password(rabbitPass);
//         });

//         // Optional: global retry on consumers hosted here (if any)
//         cfg.ConfigureEndpoints(context);
//     });
// });
builder.Services.AddMassTransit(x =>
{
    // 👇 REGISTER THE CONSUMER
    // x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });
        // Retry (3 attempts, 2s apart)
        cfg.UseMessageRetry(r =>
        {
            r.Interval(3, TimeSpan.FromSeconds(2));
        });

        // outbox on consumer side to avoid duplicate writes
        cfg.UseInMemoryOutbox();


        // 👇 CONNECT CONSUMER TO RABBIT QUEUE
        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("SearchDb",MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));

await DB.Index<Item>()
    .Key(x => x.Make, KeyType.Text)
    .Key(x => x.Model, KeyType.Text)
    .Key(x => x.Color, KeyType.Text)
    .CreateAsync();
    
app.Run();