using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Telemetry.Core
{
    public static class LogSetup
    {
        private static bool _hasRun;

        public static void Run()
        {
            if (_hasRun) return;
            var environment = ConfigurationManager.AppSettings["Environment"];
            var configFileName = string.Format("nlog-{0}.config", environment);
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
            if (!File.Exists(configFilePath))
            {
                File.Create(configFilePath);
            }
            var file = new FileTarget {FileName = configFilePath};
            var enableTraceRule = new LoggingRule("*", LogLevel.Trace, file);
            var enableErrorRule = new LoggingRule("*", LogLevel.Debug, file);
            var loggingConfig = new LoggingConfiguration();
            loggingConfig.LoggingRules.Add(enableTraceRule);
            loggingConfig.LoggingRules.Add(enableErrorRule);
            LogManager.Configuration = loggingConfig;
           
            // LogManager.Configuration = new XmlLoggingConfiguration(configFilePath, true);
            _hasRun = true;
        }
    }
}
