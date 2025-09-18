using Contatos.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Infrastructure.Time
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
