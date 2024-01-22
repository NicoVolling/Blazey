using Blazey.Communications;
using System.Diagnostics.Metrics;

namespace Blazey.Components.Table;

public static class TaskResultMessageCollectionExtension
{
    public static string X0_results_found_SG_PL(this TaskResultMessageCollection messageCollection, int EntryCount)
    {
        switch (EntryCount)
        {
            case 0:
                return "Keinen Eintrag gefunden.";

            case 1:
                return "1 Eintrag gefunden.";

            default:
                return string.Format("{0} Einträge gefunden.", EntryCount.ToString("N0"));
        }
    }
    public static string There_are_X0_entries_that_do_not_match_the_conditions_SG_PL(this TaskResultMessageCollection messageCollection, int EntryCount)
    {
        if (EntryCount == 1)
        {
            return "Es gibt einen Eintrag, der nicht den Bedingungen entspricht.";
        }
        else
        {
            return string.Format("Es gibt {0} Einträge, die nicht den Bedingungen entsprechen.", EntryCount.ToString("N0"));
        }
    }

    public static string None_of_the_X0_entries_match_the_conditions(this TaskResultMessageCollection messageCollection, int EntryCount)
    {
        return string.Format("Keiner der {0} Einträge entspricht den Bedingungen.", EntryCount.ToString("N0"));
    }

    public static string All_entries_match_the_conditions(this TaskResultMessageCollection messageCollection)
    {
        return "All Einträge entsprechen den Bedingungen.";
    }

    public static string There_is_only_one_entry(this TaskResultMessageCollection messageCollection)
    {
        return "Es gibt nur einen Eintrag.";
    }

    public static string There_is_no_entry(this TaskResultMessageCollection messageCollection)
    {
        return "Es gibt keinen Eintrag.";
    }

    public static string There_is_no_entry_matching_the_search_parameters(this TaskResultMessageCollection messageCollection)
    {
        return "Es gibt keinen Eintrag der den Suchparametern entspricht.";
    }

    public static string X0_of_X1_entries(this TaskResultMessageCollection messageCollection, int Count1, int Count2)
    {
        return string.Format("{0} / {1} Einträge.", Count1.ToString("N0"), Count2.ToString("N0"));
    }
}