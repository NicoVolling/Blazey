using Blazey.Communications;
using Blazey.Data.DataObject;

namespace Blazey.Data.Services;

public interface ISecuredDataService<T> where T : class, IBaseDataObject
{
    public bool IsChangeGranted(OperationStateProvider OperationStateProvider, T NewObject);

    public bool IsCreationGranted(OperationStateProvider OperationStateProvider, T NewObject);

    public bool IsDeletionGranted(OperationStateProvider OperationStateProvider, T Object);
}