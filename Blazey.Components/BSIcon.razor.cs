using Microsoft.AspNetCore.Components;

namespace Blazey.Components;

public partial class BSIcon : BaseComponent
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public IconClass Icon { get; set; }

    #region Icons

    [Parameter]
    public bool AddIcon { set => AutoProp(IconClass.PLUS_LG, value); get => AutoProp(IconClass.PLUS_LG); }

    [Parameter]
    public bool ChangeIcon { set => AutoProp(IconClass.PEN_FILL, value); get => AutoProp(IconClass.PEN_FILL); }

    [Parameter]
    public bool DeleteIcon { set => AutoProp(IconClass.TRASH_FILL, value); get => AutoProp(IconClass.TRASH_FILL); }

    [Parameter]
    public bool RevertIcon { set => AutoProp(IconClass.ARROW_CLOCKWISE, value); get => AutoProp(IconClass.ARROW_CLOCKWISE); }

    [Parameter]
    public bool SaveIcon { set => AutoProp(IconClass.CHECK_LG, value); get => AutoProp(IconClass.CHECK_LG); }

    #endregion Icons

    [Parameter]
    public bool me_3 { get; set; }

    private string _IconClass
    {
        get
        {
            return (Enum.GetName<IconClass>(Icon) ?? "square")
                .Replace("_0", "0")
                .Replace("_1", "1")
                .Replace("_2", "2")
                .Replace("_3", "3")
                .Replace("_4", "4")
                .Replace("_5", "5")
                .Replace("_6", "6")
                .Replace("_7", "7")
                .Replace("_8", "8")
                .Replace("_9", "9")
                .Replace("_", "-")
                .ToLower();
        }
    }

    protected bool AutoProp(IconClass Class) => Icon == Class;

    protected void AutoProp(IconClass Class, bool Value) => Icon = Value ? Class : Icon;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (me_3) { Class = (Class ?? "") + " me-3"; }
    }
}