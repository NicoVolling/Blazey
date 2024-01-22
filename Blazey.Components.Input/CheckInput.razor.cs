using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Input;

public partial class CheckInput : BaseControl<bool>
{
    private Guid id;

    [Parameter]
    public bool Switch { get; set; }

    protected async Task OnChanged(ChangeEventArgs args)
    {
        await ValueChanged.InvokeAsync((bool)(args.Value ?? false));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        id = Guid.NewGuid();
    }
}