using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine.Parsing;

namespace ConsoleTemplate
{
   internal class Worker : BackgroundService
   {
      private static ILogger<Worker> log;
      private static ILoggerFactory logFactory;
      private static IConfiguration config;
      private static StartArgs startArgs;
      private static Parser rootParser;

      public Worker(ILogger<Worker> logger, ILoggerFactory loggerFactory, IConfiguration configuration, StartArgs sArgs)
      {
         log = logger;
         logFactory = loggerFactory;
         config = configuration;
         startArgs = sArgs;
      }

      internal static void HelloWorld(string[] message)
      {
         log.LogInformation(string.Join(" ", message), ConsoleColor.Green);
      }

      protected async override Task ExecuteAsync(CancellationToken stoppingToken)
      {
         Directory.SetCurrentDirectory(Path.GetDirectoryName(System.AppContext.BaseDirectory));
         rootParser = CommandBuilder.BuildCommandLine();
         string[] args = startArgs.Args;
         if (args.Length == 0) args = new string[] { "-h" };
         int val = await rootParser.InvokeAsync(args);
         while (true)
         {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write("ct> ");
            Console.ResetColor();
            var line = Console.ReadLine();
            if (line == null)
            {
               return;
            }

            if (line.Length == 0) line = "-h";
            val = await rootParser.InvokeAsync(line);
         }
      }
   }
}
