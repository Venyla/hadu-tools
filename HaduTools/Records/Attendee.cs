using CsvHelper.Configuration.Attributes;

public record struct Attendee
{
    [Name("Vorname")]
    public string FirstName {get; set;}

    [Name("Nachname")]
    public string Lastname { get; set; }

    [Name("Status")]
    public string State { get; set; }

    [Name("Anzahl Teilnahmen")]
    public int Total { get; set; }
}