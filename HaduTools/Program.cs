using Spectre.Console;
using Spectre.Console.Cli;

AnsiConsole.Write(
    new FigletText("HADU Attendee List Tool")
        .LeftJustified()
        .Color(Color.Blue));
var app = new CommandApp<ExtractTrainingGuestsCommand>();
return app.Run(args);