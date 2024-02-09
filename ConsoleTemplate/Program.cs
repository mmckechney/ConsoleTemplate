using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ConsoleTemplate
{
   internal class Program
   {

      public static void Main(string[] args)
      {
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args)
      {
         (LogLevel level, bool set) = GetLogLevel(args);

         if (set)
         {
            System.Console.WriteLine($"Log level set to '{level.ToString()}'");
            args = new string[] { "--help" };
         }


         var builder = new HostBuilder()
             .ConfigureLogging(logging =>
             {
                logging.SetMinimumLevel(level);
                logging.AddFilter("System", LogLevel.Warning);
                logging.AddFilter("Microsoft", LogLevel.Warning);
             })
             .ConfigureServices((hostContext, services) =>
             {
                services.AddSingleton<StartArgs>(new StartArgs(args));
                services.AddHostedService<Worker>();
                services.AddSingleton<ConsoleFormatter, CustomConsoleFormatter>();
                services.AddLogging(builder =>
                {
                   builder.AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>();
                   builder.AddConsole(options =>
                   {
                      options.FormatterName = "custom";

                   });
                   builder.AddFilter("Microsoft", LogLevel.Warning);
                   builder.AddFilter("System", LogLevel.Warning);
                });
             })
             .ConfigureAppConfiguration((hostContext, appConfiguration) =>
             {
                appConfiguration.SetBasePath(System.IO.Path.GetDirectoryName(System.AppContext.BaseDirectory));
                appConfiguration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                appConfiguration.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
                appConfiguration.AddEnvironmentVariables();
             });
         return builder;
      }

      private static (LogLevel, bool) GetLogLevel(string[] args)
      {
         if (args.Contains("--debug"))
         {
            return (LogLevel.Debug, true);
         }
         else if (args.Contains("--trace"))
         {
            return (LogLevel.Trace, true);
         }
         else if (args.Contains("--info"))
         {
            return (LogLevel.Information, true);
         }
         else if (args.Contains("--warn"))
         {
            return (LogLevel.Warning, true);
         }
         else if (args.Contains("--error"))
         {
            return (LogLevel.Error, true);
         }
         else if (args.Contains("--critical"))
         {
            return (LogLevel.Critical, true);
         }
         else if (args.Contains("--default"))
         {
            return (LogLevel.Information, true);
         }
         else
         {
            return (LogLevel.Information, false);
         }
      }
   }
}
