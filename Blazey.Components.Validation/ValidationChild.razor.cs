using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Validation;

public partial class ValidationChild : BaseComponent
{
    public Func<bool> Delete = () => false;

    [Parameter]
    public Func<bool>? BindToEvaluation { get; set; } = null;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter, EditorRequired]
    public int ID { get; set; }

    [Parameter]
    public Func<bool> IsValid { get; set; } = () => true;

    [Parameter]
    public bool ShowValid { get; set; }

    [Parameter]
    public bool SummaryIgnore { get; set; }

    [CascadingParameter]
    public ValidationMaster? ValidationMaster { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (BindToEvaluation != null)
        {
            Delete = () => !BindToEvaluation();
        }

        if (ValidationMaster != null)
        {
            ValidationMaster.AddChild(this);
        }
    }
}