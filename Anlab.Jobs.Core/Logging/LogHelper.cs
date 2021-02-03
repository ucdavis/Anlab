using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using StackifyLib;

namespace Anlab.Jobs.Core.Logging
{
    public static class LogHelper {
        public static LoggerConfiguration LogConfiguration;

        public static bool _loggingSetup = false;

        public static void ConfigureLogging(IConfigurationRoot configuration) {
            if (_loggingSetup) return;

            configuration.ConfigureStackifyLogging(); // use the config setting keys

            var loggingSection = configuration.GetSection("Stackify");

            LogConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                // .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning) // uncomment this to hide EF core general info logs
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", loggingSection.GetValue<string>("AppName"))
                .Enrich.WithProperty("AppEnvironment", loggingSection.GetValue<string>("Environment"))
                .WriteTo.Stackify()
                .WriteTo.Console();

            // add in elastic search sink if the uri is valid
            if (Uri.TryCreate(loggingSection.GetValue<string>("ElasticUrl"), UriKind.Absolute, out var elasticUri))
            {
                LogConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(elasticUri)
                {
                    IndexFormat = "aspnet-anlab-{0:yyyy.MM.dd}"
                });
            }

            Log.Logger = LogConfiguration.CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Log.Fatal(e.ExceptionObject as Exception, e.ExceptionObject.ToString());
            
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Log.CloseAndFlush();

            _loggingSetup = true;
        }
    }
}
