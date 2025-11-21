using AnalyticService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticService.Repositories;

public interface IBookAnalyticsRepository
{
    Task AddAsync(BookAnalytics bookAnalytics, CancellationToken cancellationToken);
}
