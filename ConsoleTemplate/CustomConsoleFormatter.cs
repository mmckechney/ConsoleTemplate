
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.Text.RegularExpressions;
namespace ConsoleTemplate
{

   public sealed class CustomConsoleFormatter : ConsoleFormatter
   {
      
      private Dictionary<string, ConsoleColor> customColors = null;
      private ConsoleColor defaultColor;
      public CustomConsoleFormatter() : base("custom")
      {
      }
      /// <summary>
      /// Custom color dictionary to allow the use of custom strings to define output colors
      /// </summary>
      /// <param name="customColors"></param>
      public CustomConsoleFormatter(Dictionary<string, ConsoleColor> customColors, ConsoleColor defaultColor = ConsoleColor.Blue) : base("custom")
      {
         this.customColors = customColors;
         this.defaultColor = defaultColor;
      }

      public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
      {
         (var color, var level) = LogLevelShort(logEntry.LogLevel);

         var messages = logEntry.State.ToString().Split("|", StringSplitOptions.RemoveEmptyEntries);
         string parsedMessage = "";

         if (logEntry.LogLevel != LogLevel.Information)
         {
            Console.Write("[");
            Console.ForegroundColor = color;
            Console.Write($"{level}");
            Console.ResetColor();
            Console.Write("] ");
         }
         foreach (var msg in messages)
         {
            (Console.ForegroundColor, parsedMessage) = GetLogEntryColor(msg);
            Console.Write($"{parsedMessage}");
            Console.ResetColor();
         }
         Console.WriteLine();

      }
      private (ConsoleColor, string) LogLevelShort(LogLevel level)
      {
         switch (level)
         {
            case LogLevel.Trace:
               return (ConsoleColor.Blue, "TRC");
            case LogLevel.Debug:
               return (ConsoleColor.Blue, "DBG");
            case LogLevel.Information:
               return (ConsoleColor.White, "INF");
            case LogLevel.Warning:
               return (ConsoleColor.DarkYellow, "WRN");
            case LogLevel.Error:
               return (ConsoleColor.Red, "ERR");
            case LogLevel.Critical:
               return (ConsoleColor.DarkRed, "CRT");
            default:
               return (ConsoleColor.Cyan, "UNK");

         }
      }
      public (ConsoleColor color, string message) GetLogEntryColor(string message)
      {
         var color = ConsoleColor.White;
         if (message.Contains("**COLOR:"))
         {
            var colorString = message.Split("**COLOR:")[1];
            if(customColors != null && customColors.ContainsKey(colorString))
            {
               return (customColors[colorString], message.Split("**COLOR:")[0]);
            }
            else if (Enum.TryParse(colorString, true, out color))
            {
               return (color, message.Split("**COLOR:")[0]);
            }
            else
            {
               return (defaultColor, message);
            }
         }
         return (color, message);
      }

   }
   public static partial class ILoggerExtensions
   {
      /// <summary>
      /// Do define the color of the message, wrap the colored words in double curly braces, ending with a colon (:) and color name.
      /// For example log.LogInformation("this is my {{blue message:blue}} and this is my {{red message:red}}");
      /// </summary>
      /// <param name="logger"></param>
      /// <param name="message"></param>
      public static void LogInformation(this ILogger logger, string message)
      {
         logger.Log(LogLevel.Information,FormatMessages(message));
      }

      /// <summary>
      /// Do define the color of the message, wrap the colored words in double curly braces, ending with a colon (:) and color name.
      /// For example log.LogDebug("this is my {{blue message:blue}} and this is my {{red message:red}}");
      /// </summary>
      /// <param name="logger"></param>
      /// <param name="message"></param>
      public static void LogDebug(this ILogger logger, string message)
      {
         logger.Log(LogLevel.Debug, FormatMessages(message));
      }

      /// <summary>
      /// Do define the color of the message, wrap the colored words in double curly braces, ending with a colon (:) and color name.
      /// For example log.LogError("this is my {{blue message:blue}} and this is my {{red message:red}}");
      /// </summary>
      /// <param name="logger"></param>
      /// <param name="message"></param>
      public static void LogError(this ILogger logger, string message)
      {
         logger.Log(LogLevel.Error, FormatMessages(message));
      }

      /// <summary>
      /// Do define the color of the message, wrap the colored words in double curly braces, ending with a colon (:) and color name.
      /// For example log.Warning("this is my {{blue message:blue}} and this is my {{red message:red}}");
      /// </summary>
      /// <param name="logger"></param>
      /// <param name="message"></param>
      public static void LogWarning(this ILogger logger, string message)
      {
         logger.Log(LogLevel.Warning, FormatMessages(message));
      }

      /// <summary>
      /// Do define the color of the message, wrap the colored words in double curly braces, ending with a colon (:) and color name.
      /// For example log.LogCritical("this is my {{blue message:blue}} and this is my {{red message:red}}");
      /// </summary>
      /// <param name="logger"></param>
      /// <param name="message"></param>
      public static void LogCritical(this ILogger logger, string message)
      {
         logger.Log(LogLevel.Critical, FormatMessages(message));
      }

      /// <summary>
      /// Do define the color of the message, wrap the colored words in double curly braces, ending with a colon (:) and color name.
      /// For example log.LogTrace("this is my {{blue message:blue}} and this is my {{red message:red}}");
      /// </summary>
      /// <param name="logger"></param>
      /// <param name="message"></param>
      public static void LogTrace(this ILogger logger, string message)
      {
         logger.Log(LogLevel.Trace, FormatMessages(message));
      }


      private static Regex curlyBraceRegex = new Regex(@"\{(.+?):(.+?)\}");
      private static string FormatMessages(string message)
      { 
         ConsoleColor color = ConsoleColor.White;
         string[] substrings = curlyBraceRegex.Split(message);
         List<string> formattedMessages = new List<string>();
         for (int i = 0; i < substrings.Length; i++)
         {
            if (i % 3 == 0)
            {
               // This is a non-matching string
               formattedMessages.Add(substrings[i] + "**COLOR:White|");
            }
            else if (i % 3 == 1)
            {
               // This is the first matching string
               // We append the second matching string (which is at i+1) and add it to the list
               formattedMessages.Add(substrings[i] + "**COLOR:" + substrings[i + 1] + "|");
               i++; // Skip the next iteration because we've already processed the second matching string
            }
         }
         return string.Join("", formattedMessages);
      }

      public static void LogInformation(this ILogger logger, string message, ConsoleColor color)
      {
         logger.LogInformation(FormatMessage(message, color));
      }

      public static void LogDebug(this ILogger logger, string message, ConsoleColor color)
      {
         logger.LogDebug(FormatMessage(message, color));
      }

      public static void LogError(this ILogger logger, string message, ConsoleColor color)
      {
         logger.LogError(FormatMessage(message, color));
      }

      public static void LogWarning(this ILogger logger, string message, ConsoleColor color)
      {
         logger.LogWarning(FormatMessage(message, color));
      }

      public static void LogCritical(this ILogger logger, string message, ConsoleColor color)
      {
         logger.LogCritical(FormatMessage(message, color));
      }

      public static void LogTrace(this ILogger logger, string message, ConsoleColor color)
      {
         logger.LogTrace(FormatMessage(message, color));
      }
      private static string FormatMessage(string message, ConsoleColor color)
      {
         return message + " **COLOR:" + color.ToString();
      }
   }
}