using Blazey.Communications;

namespace Blazey.Data;

public static class TaskResultMessageCollectionExtension
{
    public static string Are_you_sure_you_want_to_delete_this_object(this TaskResultMessageCollection messageCollection)
    {
        return "Sind Sie sicher, dass Sie den Eintrag löschen möchten?";
    }

    public static string Deletion_aborted(this TaskResultMessageCollection taskResultMessageCollection)
    {
        return "Löschen abgebrochen.";
    }

    public static string This_object_does_not_exist(this TaskResultMessageCollection taskResultMessageCollection)
    {
        return "Der Eintrag konnte nicht gefunden werden.";
    }

    public static string You_are_not_authorized_to_perform_this_operation(this TaskResultMessageCollection taskResultMessageCollection)
    {
        return "Die Operation konnte aufgrund fehlender Berechtigungen nicht durchgeführt werden.";
    }

    public static string A_unexpected_error_occured(this TaskResultMessageCollection taskResultMessageCollection)
    {
        return "Es ist ein unerwarteter Fehler beim durchführen der Aktion aufgetreten.";
    }
}