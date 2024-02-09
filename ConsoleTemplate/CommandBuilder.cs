using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

namespace ConsoleTemplate
{
   internal class CommandBuilder
   {
      public static Parser BuildCommandLine()
      {

         var messageArg = new Argument<string[]>("message", "Message to Echo") { Arity = ArgumentArity.ZeroOrMore };
         var messageOpt = new Option<string>(new string[] { "--message", "-m" }, "Message to Echo") { IsRequired = false };


         RootCommand rootCommand = new RootCommand(description: $"Console Template project");
         rootCommand.Add(messageArg);
         //rootCommand.Add(messageOpt);
         rootCommand.Handler = CommandHandler.Create<string[]>(Worker.HelloWorld);

         var parser = new CommandLineBuilder(rootCommand)
              .UseDefaults()
              .UseHelp(ctx =>
              {
                 ctx.HelpBuilder.CustomizeLayout(_ => HelpBuilder.Default
                                    .GetLayout()
                                    .Prepend(
                                        _ => AnsiConsole.Write(new FigletText("Console Template project"))
                                    ));

              })
              .Build();

         return parser;
      }


   }
}
