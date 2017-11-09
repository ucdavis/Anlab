using Serilog;

namespace Anlab.Jobs.Core.Logging
{
    public static class LogHelper {
        public static LoggerConfiguration LogConfiguration;

        public static bool _loggingSetup = false;

        public static void ConfigureLogging() {
            if (_loggingSetup) return;

            LogConfiguration = new LoggerConfiguration()
                .WriteTo.Stackify()
                .WriteTo.Console();

            Log.Logger = LogConfiguration.CreateLogger();

            _loggingSetup = true;
        }
    }
}