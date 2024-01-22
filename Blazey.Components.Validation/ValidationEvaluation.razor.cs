using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazey.Components.Validation;

public partial class ValidationEvaluation : BaseComponent
{
    [Parameter]
    public bool AsSubmitButton { get; set; }

    [Parameter]
    public RenderFragment<bool>? ChildContent { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public bool InvalidColor { get; set; }

    [Parameter]
    public bool InvalidDisable { get; set; }

    [Parameter]
    public bool InvalidInvisible { get; set; }

    [Parameter]
    public Action<MouseEventArgs> onclick { get; set; }

    [Parameter, EditorRequired]
    public int ValidationID { get; set; }

    [CascadingParameter]
    public ValidationMaster? ValidationMaster { get; set; }

    protected string FullClass
    {
        get
        {
            bool valid = GetValidStatus();
            return
            Class + " " +
            (InvalidColor ? (valid ? "Valid" : "Invalid") : "") + " " +
            (InvalidDisable ? (valid ? "" : "disabled") : "") + " " +
            (InvalidInvisible ? (valid ? "" : "invisible") : "");
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ValidationMaster != null)
        {
            ValidationMaster.ValidationChildsChanged += ValidationMaster_ValidationChildsChanged;
        }
    }

    private bool GetValidStatus()
    {
        return ValidationMaster?.GetValidation(ValidationID) == true;
    }

    private void OnClick(MouseEventArgs MouseEventArgs)
    {
        if (GetValidStatus())
        {
            onclick.Invoke(MouseEventArgs);
        }
    }

    private void ValidationMaster_ValidationChildsChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
}