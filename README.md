# hadu-tools
Tools used for internal club efficiency.

## Extract Training Guests

The downloaded attendee list form ClubDesk can be used as input file.
To extract all trainings guests that attended the trainings within the list, use:

```csharp
dotnet run <filepath/file.csv> -s <filepaht/result-file.csv>
```
In addition the delimiter and culture of the file can be added.

```csharp
dotnet run <filepath/file.csv> -s <filepaht/result-file.csv>  -d ';' -c 'de-CH'
```
