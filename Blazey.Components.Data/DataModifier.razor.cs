using Blazey.Communications;
using Blazey.Components.Attributes;
using Blazey.Components.Validation;
using Blazey.Data.DataObject;
using Blazey.Data.Services;
using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Data;

public partial class DataModifier<T> : BaseComponent, IOperationStateProviderComponent where T : class, IBaseDataObject, new()
{
    private EditMode editMode;

    protected TaskResultMessageCollection TRMC { get; } = new();

    [Parameter]
    public string? Alt_Object_ID { get; set; }

    [QueryParameter(DefaultValue = "True")]
    public bool BackAfterCreate { get; set; }

    [Parameter]
    public RenderFragment<DataModifierState<T>>? ChildContent { get; set; }

    public T? DataObject { get; set; }

    [QueryParameter(Name = "ID")]
    public Guid? DataObject_ID { get; set; }

    [Parameter, EditorRequired]
    public BaseDataService<T> DataService { get; set; } = default!;

    [Parameter]
    public bool DisableCreation { get; set; }

    [Parameter]
    public bool DisableDeletion { get; set; }

    public EditMode EditMode 
    { 
        get => editMode; 
        set => editMode = value; 
    }

    [Parameter, EditorRequired]
    public string ListUrl { get; set; } = string.Empty;

    [QueryParameter(DefaultValue = "1")]
    public int Mode
    { 
        get => (int)EditMode; 
        set 
        { 
            if (Enum.TryParse(typeof(EditMode), value.ToString(), out object? editMode)) 
            { 
                EditMode = (EditMode)editMode; 
            } 
        } 
    }

    [Parameter]
    public Func<T, bool> ModificationButtonsCondition { get; set; } = o => true;

    [Parameter]
    public Func<T> OnCreate { get; set; } = () => new();

    [Parameter]
    public OperationStateProvider OperationState { get; set; } = new();

    [Parameter]
    public Func<T, bool> PrintButtonCondition { get; set; } = o => true;

    [Parameter]
    public bool PrintButtonEnabled { get; set; }

    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    private string? AuthorizationError { get; set; }

    private bool DeletionUnauthorized { get; set; }

    private bool Unauthorized { get; set; }

    private ValidationMaster? ValidationMaster { get; set; }

    protected async Task Delete()
    {
        OperationState.Reset();

        if (DataObject != null)
        {
            if ((await DataService.DeleteAsync(OperationState, DataObject.Id)).Success && OperationState.Completed)
            {
                if (BackAfterCreate)
                {
                    NavigateTo(ListUrl);
                }
                else
                {
                    NavigateTo(GetQuery(new (string Key, object? Value)[] { ("ID", null), (nameof(Mode), (int)EditMode.Create) }));
                }
            }
        }
        else
        {
            OperationState.AddError("Cannot delete imaginary object.");
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (DataObject != null && EditMode != EditMode.Error && DataService is ISecuredDataService<T> SecuredDataService)
        {
            OperationStateProvider OSP = new();
            AuthorizationError = null;

            bool unauthorized = false;
            bool deletionUnauthorized = false;

            if (EditMode == EditMode.Edit)
            {
                unauthorized = !SecuredDataService.IsChangeGranted(OSP, DataObject);
            }
            else
            if (EditMode == EditMode.Create)
            {
                unauthorized = !SecuredDataService.IsCreationGranted(OSP, DataObject);
            }
            else
            {
                deletionUnauthorized = !SecuredDataService.IsDeletionGranted(OSP, DataObject);
            }

            if (unauthorized != Unauthorized)
            {
                Unauthorized = unauthorized;
                AuthorizationError = OSP.ErrorMessages.FirstOrDefault();
                Refresh();
            }
            if (deletionUnauthorized != DeletionUnauthorized)
            {
                DeletionUnauthorized = deletionUnauthorized;
                Refresh();
            }
        }
    }

    protected override void OnLocationChanged()
    {
        base.OnLocationChanged();
        if (EditMode == EditMode.Create)
        {
            DataObject_ID = null;
            DataObject = OnCreate.Invoke();
        }

        if (EditMode == EditMode.View)
        {
            Unauthorized = false;
            AuthorizationError = null;
        }

        if (DataObject_ID != null && Alt_Object_ID == null)
        {
            if (DataService.TryGetById(DataObject_ID.Value, out T? result) && result != null)
            {
                DataObject = result.Clone<T>();
            }
        }
        else if (Alt_Object_ID != null)
        {
            if (DataService.TryGetById(Alt_Object_ID, out T? result) && result != null)
            {
                DataObject = result.Clone<T>();
            }
        }
        ValidationMaster?.ReEvaluate();
        InvokeAsync(StateHasChanged);
    }

    private void SaveChanges()
    {
        OperationState.Reset();

        if (DataObject == null)
        {
            OperationState.AddError("An unexpected error occurred while saving. The object to be saved may no longer exist.");
            return;
        }

        if (EditMode == EditMode.Edit || EditMode == EditMode.Create)
        {
            if (DataService.ChangeOrCreate(OperationState, DataObject).Success)
            {
                EditMode = EditMode.View;
                string query = GetQuery([("ID", DataObject.Id.ToString("N"))]);
                NavigateTo(query);
            }
        }
    }
}
