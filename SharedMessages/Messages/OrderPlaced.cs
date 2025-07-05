using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Messages;

public sealed record  OrderPlaced(Guid OrderId , int Quantity);
