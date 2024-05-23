namespace ConsoleTemplate.Logging
{
    public class CustomConsoleColors
    {
      public static Dictionary<ConsoleColor, (int, int, int)> StandardColorMapping = new Dictionary<ConsoleColor, (int, int, int)>
      {
          { ConsoleColor.Black, (0, 0, 0) },
          { ConsoleColor.DarkBlue, (0, 0, 128) },
          { ConsoleColor.DarkGreen, (0, 128, 0) },
          { ConsoleColor.DarkCyan, (0, 128, 128) },
          { ConsoleColor.DarkRed, (128, 0, 0) },
          { ConsoleColor.DarkMagenta, (128, 0, 128) },
          { ConsoleColor.DarkYellow, (128, 128, 0) },
          { ConsoleColor.Gray, (192, 192, 192) },
          { ConsoleColor.DarkGray, (128, 128, 128) },
          { ConsoleColor.Blue, (0, 0, 255) },
          { ConsoleColor.Green, (0, 255, 0) },
          { ConsoleColor.Cyan, (0, 255, 255) },
          { ConsoleColor.Red, (255, 0, 0) },
          { ConsoleColor.Magenta, (255, 0, 255) },
          { ConsoleColor.Yellow, (255, 255, 0) },
          { ConsoleColor.White, (255, 255, 255) },
      };

      public static void PrintCustomColorMap()
        {
            Console.WriteLine("Need help, use this Color Picker: https://www.bing.com/search?q=color+picker");
            //for (int i = 0; i < 256; i++)
            //{
            //    Console.Write($"\x1b[48;5;{i}m {i.ToString().PadLeft(5)} ");
            //    if (i % 20 == 0)
            //    {
            //        Console.ResetColor();
            //        Console.WriteLine();
            //    }
            //}
            Console.ResetColor();
        }



      public static string EscapedRGBColor((int r, int g, int b) rgb)
      {
         return EscapedRGBColor(rgb.r, rgb.g, rgb.b);
      }
      public static string EscapedRGBColor(int r, int g, int b)
      {
         return $"\x1b[38;2;{r};{g};{b}m";
      }
      public static string EscapedRGBColor(ConsoleColor color)
      {
         var mapped = StandardColorMapping[color];
         return EscapedRGBColor(mapped.Item1, mapped.Item2, mapped.Item3);
      }


      public static string BackgoundEscapedRGBColor(int r, int g, int b)
      {
         return $"\x1b[38;2;{r};{g};{b}m";
      }
      public static string BackgoundEscapedRGBColor((int r, int g, int b) rgb)
      {
         return BackgoundEscapedRGBColor(rgb.r, rgb.g, rgb.b);
      }
      public static string BackgoundEscapedRGBColor(ConsoleColor color)
      {
         var mapped = StandardColorMapping[color];
         return BackgoundEscapedRGBColor(mapped.Item1, mapped.Item2, mapped.Item3);
      }
      public static string ResetColor()
      {
         return "\x1b[0m";
      }
    }
}
