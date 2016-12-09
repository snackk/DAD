using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingService
{
    public interface ILogger
    {
        void logAsync(params string[] logLine);
        void log(params string[] logLine);
    }
}
