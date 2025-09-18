using Contatos.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Tests.Helper
{
    public class Clock : IClock
    {
        private readonly DateTime _now;
        public Clock(DateTime now) => _now = now;
        public DateTime UtcNow() => _now;
    }
}
