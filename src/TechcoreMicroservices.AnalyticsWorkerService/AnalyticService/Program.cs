using AnalyticService;
using AnalyticService.Consumers;
using AnalyticService.Services.HostedService;
using AnalyticService.Services.QueueService;
using AnalyticService.Settings;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

var builder = Host.CreateApplicationBuilder(args);

// Читаем настройки Kafka из конфигурационного файла
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings:Book"));

builder.Services.AddSingleton<KafkaConsumerLoop<BookResponse>>();

builder.Services.AddHostedService<QueuedHostedService>();

builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
    new DefaultBackgroundTaskQueue(capacity: 100));


var host = builder.Build();

KafkaConsumerLoop<BookResponse> consumerLoop =
    host.Services.GetRequiredService<KafkaConsumerLoop<BookResponse>>();

consumerLoop.StartConsume();

host.Run();
