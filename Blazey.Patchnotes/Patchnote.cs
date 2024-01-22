using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazey.Patchnotes;
public class Patchnote
{
    public Patchnote(string Version, DateTime Release)
    {
        this.Version = Version;
        this.Release = Release;
    }

    public List<string> Additions { get; set; } = new List<string>();

    public List<string> Bugfixes { get; set; } = new List<string>();

    public List<string> Changes { get; set; } = new List<string>();

    public DateTime Release { get; set; }

    public string Version { get; init; }

    public static Patchnote Create(string Version, DateTime Release)
    {
        return new(Version, Release);
    }

    public Patchnote AddAddition(string Description)
    {
        Additions.Add(Process(Description));
        return this;
    }

    public Patchnote AddBugfix(string Description)
    {
        Bugfixes.Add(Process(Description));
        return this;
    }

    public Patchnote AddChange(string Description)
    {
        Changes.Add(Process(Description));
        return this;
    }

    private string Process(string Description)
    {
        int count = 0;
        string newDescription = Regex.Replace(Description, "\"", m => {
            count++;
            return count % 2 == 1 ? "\"<span class=\"font-monospace\">" : "</span>\"";
        });
        return newDescription;
    }
}
