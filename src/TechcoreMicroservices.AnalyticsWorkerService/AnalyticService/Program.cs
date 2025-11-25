using AnalyticService;
using AnalyticService.Consumers;
using AnalyticService.Repositories;
using AnalyticService.Services.HostedService;
using AnalyticService.Services.QueueService;
using AnalyticService.Settings;
using MongoDB.Driver;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

var builder = Host.CreateApplicationBuilder(args);
{
    builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings:Book"));
    builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));

    builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder
        .Configuration["MongoSettings:MongoConnectionString"]));

    builder.Services.AddSingleton<IBookAnalyticsRepository, BookAnalyticsRepository>();

    builder.Services.AddHostedService<QueuedHostedService>();

    builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
        new DefaultBackgroundTaskQueue(capacity: 100));

    builder.Services.AddSingleton<KafkaConsumerLoop>();
}

var host = builder.Build();
{
    KafkaConsumerLoop consumerLoop =
    host.Services.GetRequiredService<KafkaConsumerLoop>();

    consumerLoop.StartConsume();

    host.Run();
}
