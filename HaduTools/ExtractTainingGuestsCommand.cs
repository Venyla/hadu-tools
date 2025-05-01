using System.ComponentModel;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

internal sealed class ExtractTrainingGuestsCommand : Command<ExtractTrainingGuestsCommand.Settings>
{
    const string _defaultReadDelimiter = ";";
    const string _defaultWriteDelimiter = ",";
    const string _defaultCultureCode = "de-CH";

    public sealed class Settings : CommandSettings
    {
        [Description("Path to attendance sheet downladed from ClubDesk.")]
        [CommandArgument(0, "[filePath]")]
        public string? FilePath { get; init; }

        [Description("Path to write result file to.")]
        [CommandOption("-s|--save")]
        public string? ResultFilePath { get; init; }

        [Description($"The delimiter used to separate fields. Default is {_defaultReadDelimiter}")]
        [CommandOption("-d|--delimiter")]
        public string? Delimiter { get; init; }

        [Description($"Culture code used to read and write file. Default is {_defaultCultureCode}")]
        [CommandOption("-c|--culture")]
        public string? CultureCode { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var filePath = settings.FilePath ?? Directory.GetCurrentDirectory();
        var resultFilePath = settings.ResultFilePath ?? Directory.GetCurrentDirectory();
        var readDelimiter = settings.Delimiter ?? _defaultReadDelimiter;
        var writeDelimiter = settings.Delimiter ?? _defaultWriteDelimiter;
        var encoding = Encoding.Default;
        var cultureInfo = new CultureInfo(settings.CultureCode ?? _defaultCultureCode);

        AnsiConsole.MarkupLine($"opening file: {filePath}, save to: {resultFilePath}");

        var records = GetRecords(filePath, readDelimiter, encoding, cultureInfo);

        var filteredRecords = records
            .Where(x => x.Total > 0 && x.State == "Trainingsgast")
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.Lastname)
            .ToList();

        PrintRecords(filteredRecords);

        if (!string.IsNullOrEmpty(resultFilePath))
        {
            WriteRecordstoResultFilePath(filteredRecords, resultFilePath, writeDelimiter, encoding, CultureInfo.InvariantCulture);
        }

        return 0;
    }

    private static IEnumerable<Attendee> GetRecords(string filePath, string delimiter, Encoding encoding, CultureInfo cultureInfo)
    {
        var bad = new List<string>();
        var config = new CsvConfiguration(cultureInfo)
        {
            Delimiter = delimiter,
            Encoding = encoding,
            BadDataFound = context => bad.Add(context.RawRecord),
            MissingFieldFound = null
        };

        var reader = new StreamReader(filePath);
        var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();

        return csv.GetRecords<Attendee>();
    }

    private static void PrintRecords(List<Attendee> filteredRecords)
    {
        var table = new Table();

        table.AddColumn(nameof(Attendee.FirstName));
        table.AddColumn(nameof(Attendee.Lastname));
        table.AddColumn(nameof(Attendee.State));
        table.AddColumn(nameof(Attendee.Total));

        foreach (var record in filteredRecords)
        {
            table.AddRow(record.FirstName, record.Lastname, record.State, record.Total.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[bold]Number of records: [/] [bold green]{filteredRecords.Count}[/]");
    }

    private static void WriteRecordstoResultFilePath(IEnumerable<Attendee> filteredRecords, string resultFilePath, string delimiter, Encoding encoding, CultureInfo cultureInfo)
    {
        var config = new CsvConfiguration(cultureInfo)
        {
            Encoding = encoding,
            Delimiter = delimiter
        };
        var writer = new StreamWriter(resultFilePath);
        var csvWriter = new CsvWriter(writer, config);

        csvWriter.WriteRecords(filteredRecords);
        csvWriter.Flush();
    }
}
