using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using StackifyLib;

namespace Anlab.Jobs.Core.Logging
{
    public static class LogHelper {
        public static LoggerConfiguration LogConfiguration;

        public static bool _loggingSetup = false;

        public static void ConfigureLogging(IConfigurationRoot configuration) {
            if (_loggingSetup) return;

            configuration.ConfigureStackifyLogging(); // use the config setting keys

            LogConfiguration = new LoggerConfiguration()
                .WriteTo.Stackify()
                .WriteTo.Console();

            Log.Logger = LogConfiguration.CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Log.Fatal(e.ExceptionObject as Exception, e.ExceptionObject.ToString());
            
            _loggingSetup = true;
        }
    }
}