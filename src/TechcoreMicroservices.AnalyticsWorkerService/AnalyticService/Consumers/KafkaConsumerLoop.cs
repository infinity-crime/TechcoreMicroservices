using AnalyticService.Deserializers;
using AnalyticService.Models;
using AnalyticService.Repositories;
using AnalyticService.Services.QueueService;
using AnalyticService.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

namespace AnalyticService.Consumers;

public class KafkaConsumerLoop : IDisposable
{
    private readonly ILogger<KafkaConsumerLoop> _logger;

    private readonly IBackgroundTaskQueue _taskQueue;

    private readonly IConsumer<string, BookResponse> _consumer;
    private readonly string _topic;

    private readonly IBookAnalyticsRepository _repository;

    private readonly CancellationToken _cancellationToken;

    public KafkaConsumerLoop(ILogger<KafkaConsumerLoop> logger,
        IBackgroundTaskQueue taskQueue,
        IBookAnalyticsRepository bookAnalyticsRepository,
        IHostApplicationLifetime appLifetime,
        IOptions<KafkaSettings> options)
    {
        _logger = logger;

        _taskQueue = taskQueue;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = options.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, BookResponse>(consumerConfig)
            .SetValueDeserializer(new DefaultKafkaDeserializer<BookResponse>())
            .Build();

        _topic = options.Value.Topic;

        _repository = bookAnalyticsRepository;

        _cancellationToken = appLifetime.ApplicationStopping;
    }

    public void StartConsume()
    {
        _consumer.Subscribe(_topic);
        Task.Run(() => ConsumeLoop(_cancellationToken));
    }

    private async Task ConsumeLoop(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Kafka consumer loop for topic {Topic}", _topic);

        try
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);

                    Func<CancellationToken, ValueTask> workItem = async token =>
                    {
                        _logger.LogInformation($"Consumed message (ConsumeLoop Method) at {cr.TopicPartitionOffset} with key: {cr.Message.Key}");

                        var newAnalytics = new BookAnalytics
                        {
                            Id = Guid.NewGuid(),
                            BookId = cr.Message.Value.Id,
                            Title = cr.Message.Value.Title,
                            Year = cr.Message.Value.Year
                        };

                        await _repository.AddAsync(newAnalytics, token);

                        _logger.LogInformation($"Added analytics: BookTitle - {newAnalytics.Title}");
                    };

                    await _taskQueue.QueueBackgroundWorkItemAsync(workItem);
                }
                catch(OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Kafka consumer loop is stopping due to cancellation request.");

                    break;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during Kafka consumption.");
                }
            }
        }
        finally
        {
            _logger.LogInformation("Closing Kafka consumer for topic {Topic}", _topic);
            _consumer.Close();
        }
    }

    public void Dispose()
    {
        _consumer?.Dispose();
    }
}
