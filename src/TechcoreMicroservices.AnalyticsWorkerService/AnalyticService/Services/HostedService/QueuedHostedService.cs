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

namespace AnalyticService.Services.HostedService;

public class QueuedHostedService : BackgroundService
{
    private readonly ILogger<QueuedHostedService> _logger;

    private readonly IBackgroundTaskQueue _taskQueue;

    public QueuedHostedService(ILogger<QueuedHostedService> logger, 
        IBackgroundTaskQueue taskQueue)
    {
        _logger = logger;
        _taskQueue = taskQueue;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer is starting.");

        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred executing task work item.");
            throw;
        }
    }
}
