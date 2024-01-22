namespace Blazey.Data.DataObject;

public interface IInputDataObjectProvider<T> : IBaseInputDataObjectProvier where T : class
{
    public T InputDataObject { get; set; }
}