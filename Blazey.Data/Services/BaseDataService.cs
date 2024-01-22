using Blazey.Communications;
using Blazey.Data.DataObject;
using Microsoft.JSInterop;

namespace Blazey.Data.Services;

public abstract class BaseDataService<T> where T : class, IBaseDataObject
{
    private readonly IJSRuntime jSRuntime;

    public BaseDataService(BaseDataHandler<T> DataHandler, IJSRuntime jSRuntime)
    {
        this.DataHandler = DataHandler;
        this.jSRuntime = jSRuntime;
        this.TaskResultMessageCollection = new();
    }

    public BaseDataHandler<T> DataHandler { get; }

    public TaskResultMessageCollection TaskResultMessageCollection { get; }

    public OperationStateProvider Change(OperationStateProvider OperationState, Guid Id, T NewObject)
    {
        if (TryGetById(Id, out T? ExistingObject, true) && ExistingObject != null)
        {
            if (!Authorize_Write(ExistingObject, NewObject)) { return OperationState.AddError(TaskResultMessageCollection.You_are_not_authorized_to_perform_this_operation()); }

            if (OnChange(OperationState, Id, ExistingObject!, NewObject).Success)
            {
                try
                {
                    DataHandler.Change(Id, NewObject);
                    DataHandler.SaveChanges();
                    ResetInputDataObject(NewObject);
                    return OperationState.Complete();
                }
                catch (Exception ex)
                {
                    return OperationState.AddError(ex.Message);
                }
            }
            else
            {
                return OperationState;
            }
        }
        else
        {
            return OperationState.AddError(TaskResultMessageCollection.This_object_does_not_exist());
        }
    }

    public OperationStateProvider ChangeOrCreate(OperationStateProvider OperationState, T Object)
    {
        if (TryGetById(Object.Id, out T? ExistingObject))
        {
            return Change(OperationState, Object.Id, Object);
        }
        else
        {
            return Create(OperationState, Object);
        }
    }

    public int Count()
    {
        return OnGet().Count();
    }

    public int Count(Func<IQueryable<T>, IQueryable<T>> ObjectFilter)
    {
        return ObjectFilter.Invoke(OnGet()).Count();
    }

    public OperationStateProvider Create(OperationStateProvider OperationState, T NewObject)
    {
        if (!Authorize_Write(null, NewObject)) { return OperationState.AddError(TaskResultMessageCollection.You_are_not_authorized_to_perform_this_operation()); }

        if (OnCreate(OperationState, NewObject).Success)
        {
            try
            {
                DataHandler.Create(NewObject);
                DataHandler.SaveChanges();
                ResetInputDataObject(NewObject);
                return OperationState.Complete();
            }
            catch (Exception ex)
            {
                return OperationState.AddError(ex.Message);
            }
        }
        else
        {
            return OperationState;
        }
    }

    public async Task<OperationStateProvider> DeleteAsync(OperationStateProvider OperationState, Guid Id, bool NoConfirmation = false)
    {
        if (!NoConfirmation)
        {
            if (!await GetConfirmationAsync(TaskResultMessageCollection.Are_you_sure_you_want_to_delete_this_object()))
            {
                return OperationState.AddWarning(TaskResultMessageCollection.Deletion_aborted());
            }
        }

        if (TryGetById(Id, out T? ExistingObject) && ExistingObject != null)
        {
            if (!Authorize_Delete(ExistingObject)) { return OperationState.AddError(TaskResultMessageCollection.You_are_not_authorized_to_perform_this_operation()); }

            if (OnDelete(OperationState, Id, ExistingObject!).Success)
            {
                try
                {
                    DataHandler.Delete(Id);
                    DataHandler.SaveChanges();
                    return OperationState.Complete();
                }
                catch (Exception ex)
                {
                    return OperationState.AddError(ex.Message);
                }
            }
            else
            {
                return OperationState;
            }
        }
        else
        {
            return OperationState.AddError(TaskResultMessageCollection.This_object_does_not_exist());
        }
    }

    public IQueryable<T> Get(bool ReadOnly = false)
    {
        return Authorize_Read(OnGet(ReadOnly));
    }

    public void ResetInputDataObject(T Object)
    {
        if (Object is IBaseInputDataObjectProvier IBIDOP)
        {
            IBIDOP.Reset();
        }
    }

    public bool TryGetById(Guid Id, out T? Result, bool ReadOnly = false) => DataHandler.TryGetById(Get(ReadOnly), Id, out Result);

    public bool TryGetById(string ID, out T? Result, bool ReadOnly = false) => DataHandler.TryGetById(Get(ReadOnly), ID, out Result);

    protected virtual bool Authorize_Delete(T Object) => true;

    protected virtual IQueryable<T> Authorize_Read(IQueryable<T> List) => List;

    protected virtual bool Authorize_Write(T? Object, T NewObject) => true;

    protected bool GetConfirmation(string Message)
    {
        return GetConfirmationAsync(Message).Result;
    }

    protected async Task<bool> GetConfirmationAsync(string Message)
    {
        return await jSRuntime.InvokeAsync<bool>("confirm", Message);
    }

    protected virtual OperationStateProvider OnChange(OperationStateProvider OperationState, Guid Id, T ExistingObject, T NewObject) => OperationState;

    protected virtual OperationStateProvider OnCreate(OperationStateProvider OperationState, T NewObject) => OperationState;

    protected virtual OperationStateProvider OnDelete(OperationStateProvider OperationState, Guid Id, T ExistingObject) => OperationState;

    protected virtual IQueryable<T> OnGet(bool ReadOnly = false) => DataHandler.Get(ReadOnly);

    protected bool TryGetById(IQueryable<T> Objects, Guid Id, out T? Result) => DataHandler.TryGetById(Objects, Id, out Result);
}