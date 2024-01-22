namespace Blazey.Data.DataObject;

public class BaseDataObject : IBaseDataObject
{
    public Guid Id { get; set; }

    public T Clone<T>() where T : class, IBaseDataObject
    {
        return (T)MemberwiseClone();
    }
}