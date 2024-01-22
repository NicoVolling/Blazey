using Blazey.Components.Attributes;
using Blazey.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using System.Reflection;

namespace Blazey.Components;

public partial class BaseComponent : ComponentBase
{
    private Guid? id;

    public Guid Id { get { if (id == null) { id = Guid.NewGuid(); } return id.Value; } }

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [CascadingParameter(Name = "PageComponent")]
    public BaseComponent? PageComponent { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected NavigationService NavigationService { get; set; } = default!;

    protected virtual bool QueryParametersEnabled { get => true; }

    protected string Url => NavigationManager.Uri;

    [CascadingParameter]
    private Task<AuthenticationState> _authenticationStateTask { get; set; } = default!;

    private List<BaseComponent> ChildComponents { get; } = new();

    public void ParseQuery(string? AbsoluteQuery = null)
    {
        Uri uri = AbsoluteQuery != null ? new Uri(AbsoluteQuery) : NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        foreach (var PropertyInfo in this.GetType().GetProperties().Where(o => o.CanWrite))
        {
            if (PropertyInfo.GetCustomAttribute<QueryParameterAttribute>() is QueryParameterAttribute qpa)
            {
                if (PropertyInfo.GetSetMethod(true) is MethodInfo MI && MI.IsPublic)
                {
                    object? value = null;

                    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(GetQueryName(this, qpa, PropertyInfo), out StringValues Raw))
                    {
                        try
                        {
                            if (PropertyInfo.PropertyType == typeof(DateTime?))
                            {
                                value = DateTime.Parse(Raw.ToString());
                            }
                            else if (PropertyInfo.PropertyType == typeof(double?))
                            {
                                value = double.Parse(Raw.ToString());
                            }
                            else if (PropertyInfo.PropertyType == typeof(int?))
                            {
                                value = int.Parse(Raw.ToString());
                            }
                            else if (PropertyInfo.PropertyType == typeof(Guid?))
                            {
                                value = Guid.Parse(Raw.ToString());
                            }
                            else
                            {
                                value = Convert.ChangeType(Raw.ToString(), PropertyInfo.PropertyType);
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (value == null && qpa.DefaultValue != null)
                    {
                        try
                        {
                            value = Convert.ChangeType(qpa.DefaultValue, PropertyInfo.PropertyType);
                        }
                        catch
                        {
                        }
                    }

                    if (value == null && PropertyInfo.PropertyType.IsValueType)
                    {
                        value = Activator.CreateInstance(PropertyInfo.PropertyType);
                    }

                    MI.Invoke(this, new object?[] { value });
                }
            }
        }
    }

    public void Refresh()
    {
        InvokeAsync(StateHasChanged);
    }

    protected bool GetConfirmation(string Message)
    {
        return GetConfirmationAsync(Message).Result;
    }

    protected async Task<bool> GetConfirmationAsync(string Message)
    {
        return await JS.InvokeAsync<bool>("confirm", Message);
    }

    protected string GetQuery(params (string Key, object? Value)[] changes)
    {
        Dictionary<string, object?> values = new();

        BaseComponent ThisComponent = PageComponent ?? this;
        List<BaseComponent> components = new(ThisComponent.ChildComponents)
        {
            ThisComponent
        };

        foreach (BaseComponent BC in components)
        {
            if (!BC.QueryParametersEnabled) { continue; }
            foreach (var PropertyInfo in BC.GetType().GetProperties().Where(o => o.CanWrite))
            {
                if (PropertyInfo.GetCustomAttribute<QueryParameterAttribute>() is QueryParameterAttribute qpa)
                {
                    string QueryName = GetQueryName(BC, qpa, PropertyInfo);

                    if (PropertyInfo.GetGetMethod(true) is MethodInfo MI && MI.IsPublic)
                    {
                        object? value;

                        if (changes.FirstOrDefault(o => o.Key == QueryName) is var result && result.Key != null)
                        {
                            value = result.Value;
                        }
                        else
                        {
                            value = MI.Invoke(BC, null);
                            if (value != null && PropertyInfo.PropertyType.IsValueType && value.Equals(Activator.CreateInstance(PropertyInfo.PropertyType)))
                            {
                                value = null;
                            }
                        }

                        if (qpa.DefaultValue != null)
                        {
                            try
                            {
                                if (value?.ToString() == qpa.DefaultValue)
                                {
                                    bool added = values.TryAdd(QueryName, null);
                                    if(!added)
                                    {
                                        values[QueryName] = null;
                                    }
                                    continue;
                                }
                            }
                            catch
                            {
                            }
                        }
                        {

                            bool added = values.TryAdd(QueryName, value);
                            if (!added)
                            {
                                values[QueryName] = value;
                            }
                        }
                    }
                }
            }
        }

        string url = NavigationManager.GetUriWithQueryParameters(values);

        return url;
    }

    protected void NavigateTo(string uri, bool forceLoad = false, bool replace = false) => NavigationManager.NavigateTo(uri, forceLoad, replace);

    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavigationService.NeedRefresh += NavigationService_NeedRefresh;
        NavigationService.LocationChanged += NavigationService_LocationChanged;
        NavigationService_LocationChanged(this, EventArgs.Empty);
        AddChildComponent();
    }

    protected virtual void OnLocationChanged()
    {
        this.Refresh();
    }

    private void AddChildComponent()
    {
        PageComponent?.ChildComponents.Add(this);
    }

    private string GetQueryName(BaseComponent BC, QueryParameterAttribute QPA, PropertyInfo Property)
    {
        if (QPA.Name != null) { return QPA.Name; }
        if (QPA.NameSuffixPropertyName != null)
        {
            if (BC.GetType().GetProperty(QPA.NameSuffixPropertyName) is PropertyInfo PI && PI.GetGetMethod() is MethodInfo MI)
            {
                return Property.Name + "_" + MI.Invoke(BC, new object[0]);
            }
        }
        return Property.Name;
    }

    private void NavigationService_LocationChanged(object? sender, EventArgs e)
    {
        ParseQuery();

        OnLocationChanged();
    }

    private void NavigationService_NeedRefresh(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}