using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Validation;

public partial class ValidationMaster : BaseComponent
{
    public event EventHandler? ValidationChildsChanged;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public bool IsValid
    {
        get
        {
            var invalidList = ValidationChilds.Where(o => !o.Delete() && !o.SummaryIgnore).Where(o => !o.IsValid());
            InvalidList = $"{DateTime.Now}: {string.Join(',', invalidList.Select(o => o.ID))}";
            return !invalidList.Any();
        }
    }

    [Parameter]
    public EventCallback<string> ValidChanged { get; set; }

    protected string? InvalidList { get; set; }

    protected List<ValidationChild> ValidationChilds { get; set; } = new();

    public void AddChild(ValidationChild child)
    {
        if (!ValidationChilds.Contains(child))
        {
            ValidationChilds.Add(child);
            OnValidationChildsChanged();
        }
    }

    public bool GetValidation(int ID)
    {
        if (ID == -2)
        {
            return true;
        }
        if (ID == -1)
        {
            return IsValid;
        }
        return ValidationChilds.Where(o => o.ID == ID).Where(o => !o.Delete()).All(o => o.IsValid());
    }

    public void ReEvaluate()
    {
        InvokeAsync(StateHasChanged);
        OnValidationChildsChanged();
    }

    protected void OnValidationChildsChanged()
    {
        if (ValidationChildsChanged != null)
        {
            ValidationChildsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override bool ShouldRender()
    {
        bool ShouldRender = base.ShouldRender();
        if (ShouldRender)
        {
            ValidationChilds.RemoveAll(o => o.Delete());
        }
        return ShouldRender;
    }
}