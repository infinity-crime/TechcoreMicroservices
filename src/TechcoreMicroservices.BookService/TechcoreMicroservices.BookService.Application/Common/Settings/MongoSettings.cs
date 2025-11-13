using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Application.Common.Settings;

public class MongoSettings
{
    public string MongoConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
