using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Kafka;

public interface IKafkaProducer : IDisposable
{
    Task ProduceAsync(BookResponse message, CancellationToken cancellationToken = default);
}
