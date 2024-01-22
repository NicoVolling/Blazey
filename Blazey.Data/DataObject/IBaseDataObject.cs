namespace Blazey.Data.DataObject;

public interface IBaseDataObject
{
    public Guid Id { get; set; }

    public T Clone<T>() where T : class, IBaseDataObject;
}