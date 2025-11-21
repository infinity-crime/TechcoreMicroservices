using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticService.Extensions;

public static class AddKafkaExtension
{
    public static IServiceCollection AddKafkaConsumer<TMessage>(this IServiceCollection services)
    {
        

        return services;
    }
}
