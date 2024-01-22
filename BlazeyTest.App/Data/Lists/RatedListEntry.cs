using Blazey.Data.DataObject;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeyTest.Application.Data.Lists;

public class RatedListEntry : BaseDataObject, IBaseDataObject
{
    public string? Link { get; set; }

    [NotMapped]
    public double Rating { get => Ratings.Any() ? Ratings.Average(o => o.Rating) : 0; }

    public List<RatedListEntryRating> Ratings { get; set; } = new();

    public string? Title { get; set; }
}
