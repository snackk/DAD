
using LoggingService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM
{
    class Logger : MarshalByRefObject, ILogger
    {
        private string LogFile = PuppetSettings.Default.LogFile;
        public delegate void AsyncMethodCaller(params string[] logLines);

        public Logger()
        {
            using (StreamWriter sw = File.CreateText(LogFile))
            {
                sw.Write("");
            }
        }
        public void logAsync(params string[] logLine)
        {
            var v = new AsyncMethodCaller(log);
            v.BeginInvoke(logLine, null, null);
        }

        public void log(params string[] logLine)
        {
            lock (LogFile)
            {
                using (StreamWriter sw = File.AppendText(LogFile))
                {
                    foreach (string s in logLine)
                    {
                        sw.WriteLine(s);
                    }
                }
            }
        }
    }
}
