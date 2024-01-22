using Blazey.Data.DataObject;

namespace BlazeyTest.Application.Data.Lists;

public class RatedListEntryRating : BaseDataObject, IBaseDataObject
{
    public int Rating { get; set; }

    public Guid? UserId { get; set; }
}
