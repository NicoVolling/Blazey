using Blazey.Data.DataObject;

namespace BlazeyTest.Application.Data.Lists;

public class RatedList : BaseDataObject, IBaseDataObject
{
    public string? Description { get; set; }

    public List<RatedListEntry> Entries { get; set; } = new();

    public string? Route { get; set; }

    public string? Title { get; set; }
}