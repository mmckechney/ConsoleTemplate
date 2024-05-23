using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using cc = ConsoleTemplate.Logging.CustomConsoleColors;
namespace ConsoleTemplate.Logging
{

   public sealed class CustomConsoleFormatter : ConsoleFormatter
   {
#if WINDOWS
      [DllImport("kernel32.dll")]
      public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

      // Import other necessary functions
      [DllImport("kernel32.dll")]
      public static extern IntPtr GetStdHandle(int nStdHandle);

      [DllImport("kernel32.dll")]
      public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

      // Constants for console mode flags
      private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
#endif

      private Dictionary<string, (int r, int g, int b)> customColors = null;
      private ConsoleColor defaultColor;
      public CustomConsoleFormatter() : base("custom")
      {
#if WINDOWS
         IntPtr hOutput = GetStdHandle(-11); // Get the standard output handle

         // Enable virtual terminal processing
         uint dwMode;
         GetConsoleMode(hOutput, out dwMode);
         dwMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
         SetConsoleMode(hOutput, dwMode);
#endif
      }
      /// <summary>
      /// Custom color dictionary to allow the use of custom strings to define output colors
      /// </summary>
      /// <param name="customColors"></param>
      public CustomConsoleFormatter(Dictionary<string, (int r, int g, int b)> customColors, ConsoleColor defaultColor = ConsoleColor.Blue) : base("custom")
      {
         this.customColors = customColors;
         this.defaultColor = defaultColor;

      }

      public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
      {
         (var levelColor, var level) = LogLevelShort(logEntry.LogLevel);

         var messages = logEntry.State.ToString().Split("|", StringSplitOptions.RemoveEmptyEntries);
         string escapedColor = "";
         string parsedMessage = "";

         if (logEntry.LogLevel != LogLevel.Information)
         {
            Console.Write($"[{levelColor}{level}{cc.ResetColor()}] ");
         }
         foreach (var msg in messages)
         {
            (escapedColor, parsedMessage) = GetLogEntryColor(msg);
            Console.Write($"{escapedColor}{parsedMessage}{cc.ResetColor()}");
         }
         Console.WriteLine();


      }
      private (string escapedColorString, string levelIndicator) LogLevelShort(LogLevel level)
      {
         switch (level)
         {
            case LogLevel.Trace:
               return (cc.EscapedRGBColor(ConsoleColor.Blue), "TRC");
            case LogLevel.Debug:
               return (cc.EscapedRGBColor(ConsoleColor.Blue), "DBG");
            case LogLevel.Information:
               return (cc.EscapedRGBColor(ConsoleColor.White), "INF");
            case LogLevel.Warning:
               return (cc.EscapedRGBColor(ConsoleColor.DarkYellow), "WRN");
            case LogLevel.Error:
               return (cc.EscapedRGBColor(ConsoleColor.Red), "ERR");
            case LogLevel.Critical:
               return (cc.EscapedRGBColor(ConsoleColor.DarkRed), "CRT");
            default:
               return (cc.EscapedRGBColor(ConsoleColor.Cyan), "UNK");

         }
      }

      private void PrintConsoleColors()
      {
         foreach (var item in CustomConsoleColors.StandardColorMapping)
         {
            Console.WriteLine($"{cc.BackgoundEscapedRGBColor(item.Value)}{item.Key.ToString().PadRight(12)} - {item.Value} {cc.ResetColor()}");

         }
      }

      private Regex rgbRegex = new Regex(@"\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)");
      public (string escapedColorString, string message) GetLogEntryColor(string message)
      {
         var color = ConsoleColor.White;
         if (message.Contains("**COLOR:"))
         {
            var colorString = message.Split("**COLOR:")[1];
            if (rgbRegex.IsMatch(colorString))
            {
               var match = rgbRegex.Match(colorString);
               var r = int.Parse(match.Groups[1].Value);
               var g = int.Parse(match.Groups[2].Value);
               var b = int.Parse(match.Groups[3].Value);
               return (cc.EscapedRGBColor(r, g, b), message.Split("**COLOR:")[0]);
            }
            else if (customColors != null && customColors.ContainsKey(colorString))
            {
               return (cc.EscapedRGBColor(customColors[colorString]), message.Split("**COLOR:")[0]);
            }
            else if (Enum.TryParse(colorString, true, out color))
            {
               return (cc.EscapedRGBColor(color), message.Split("**COLOR:")[0]);
            }
            else
            {
               return (cc.EscapedRGBColor(defaultColor), message);
            }

         }
         return (cc.EscapedRGBColor(color), message);
      }


   }
}
