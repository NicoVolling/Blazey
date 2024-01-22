using Blazey.Communications;
using Blazey.Data.DataObject;

namespace Blazey.Components.Data;

public record DataModifierState<T>(T DataObject, EditMode EditMode, OperationStateProvider OperationState) where T : class, IBaseDataObject, new();
