using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookOrderService.Contracts.Commands;

public record BookAvailabilityResponse(bool IsSuccess);
