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
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

namespace TechcoreMicroservices.BookService.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, BookResponse> _producer;
    private readonly string _topic;

    public KafkaProducer(IOptions<KafkaSettings> options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };

        _producer = new ProducerBuilder<string, BookResponse>(config)
            .SetValueSerializer(new KafkaSerializer<BookResponse>())
            .Build();

        _topic = options.Value.Topic;
    }

    public async Task ProduceAsync(BookResponse message, CancellationToken cancellationToken = default)
    {
        await _producer.ProduceAsync(_topic, new Message<string, BookResponse>()
        {
            Key = message.Id.ToString(),
            Value = message
        }, 
        cancellationToken);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
