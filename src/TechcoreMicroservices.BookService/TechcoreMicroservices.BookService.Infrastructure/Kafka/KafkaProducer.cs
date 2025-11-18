using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Kafka;
using Confluent.Kafka;
using TechcoreMicroservices.BookService.Infrastructure.Kafka.Serializers;
using Microsoft.Extensions.Options;
using TechcoreMicroservices.BookService.Application.Common.Settings;

namespace TechcoreMicroservices.BookService.Infrastructure.Kafka;

public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly string _topic;

    public KafkaProducer(IOptions<KafkaSettings> options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };

        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new KafkaSerializer<TMessage>())
            .Build();

        _topic = options.Value.Topic;
    }

    public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await _producer.ProduceAsync(_topic, new Message<string, TMessage>()
        {
            Key = "key1",
            Value = message
        }, 
        cancellationToken);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
