Confluent.Kafka.Streams / KafkaStreams.NET — это не “консьюмер + цикл + репозиторий”, 
а потоковый вычислительный граф (Topology), который работает поверх Kafka: по факту мы работаем 
не с сообщениями, а с непрерывными потоками данных (KStream / KTable); не управляем вручную партициями,
сдвигами, конкурентностью и тд.

Вместо этого описываем логику обработки: фильтрации, агрегации, оконные функции, join'ы...
Kafka сама распределяет партиции между инстансами , синхронизирует состояние, а также масштабирует обработку.

Псевдокод, чтобы описать, как бы выглядела обработка:
var builder = new StreamBuilder();

// Входной поток
var bookViews = builder.Stream<string, BookResponse>("book_views");

// Группировка + агрегация
var viewCounts = bookViews
    .GroupBy((key, value) => value.Id.ToString())
    .Count(Materialized<string, long, IKeyValueStore>("book-view-counts"));

// Вывод в другой топик
viewCounts.ToStream().To("book_views_aggregated");

// Или запись прямо в Mongo (как у нас сейчас)
viewCounts.Foreach((bookId, count) =>
{
    analyticsRepository.UpdateViewCount(bookId, count);
});

Kafka Streams хорош для статистики, метрик, аналитики событий, ОКОННЫХ вычислений. Например: "Сколько просмотров за 5 минут",
"ТОП-10 книг по просмотрам за день", "Уникальные пользователи на книгу" и многое другое. Это делается одной строкой
в Streams с окнами: .WindowedBy(TimeWindows.Of(TimeSpan.FromMinutes(5))) А в обычном consumer это превращается в
сложную логику с таймерами, кэшами и потоками.