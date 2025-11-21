using AnalyticService.Deserializers;
using AnalyticService.Services.QueueService;
using AnalyticService.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticService.Consumers;

public class KafkaConsumerLoop<TMessage> : IDisposable
{
    private readonly ILogger<KafkaConsumerLoop<TMessage>> _logger;

    private readonly IBackgroundTaskQueue _taskQueue;

    private readonly IConsumer<string, TMessage> _consumer;
    private readonly string _topic;

    private readonly CancellationToken _cancellationToken;

    public KafkaConsumerLoop(ILogger<KafkaConsumerLoop<TMessage>> logger,
        IBackgroundTaskQueue taskQueue,
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

        _consumer = new ConsumerBuilder<string, TMessage>(consumerConfig)
            .SetValueDeserializer(new DefaultKafkaDeserializer<TMessage>())
            .Build();

        _topic = options.Value.Topic;

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

                        await ValueTask.CompletedTask;
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
