using Blazey.Communications;

namespace Blazey.Components.Data;

public static class TaskResultMessageCollectionExtension
{
    public static string Error(this TaskResultMessageCollection messageCollection)
    {
        return "Fehler";
    }

    public static string Warning(this TaskResultMessageCollection messageCollection)
    {
        return "Warnung";
    }
}
