using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazey.Data.EntityFramework;

namespace Blazey.Data.Services;

public class NavigationService
{
    private readonly IAppDbContext? applicationDbContext;

    private readonly IJSRuntime jSRuntime;

    private readonly NavigationManager navigationManager;

    public NavigationService(IAppDbContext? ApplicationDbContext, NavigationManager NavigationManager, IJSRuntime JSRuntime)
    {
        applicationDbContext = ApplicationDbContext;
        navigationManager = NavigationManager;
        jSRuntime = JSRuntime;
        NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        NavigationManager.RegisterLocationChangingHandler(ConfirmInternalNavigation);
    }

    public NavigationService(NavigationManager NavigationManager, IJSRuntime JSRuntime)
    {
        navigationManager = NavigationManager;
        jSRuntime = JSRuntime;
        NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        NavigationManager.RegisterLocationChangingHandler(ConfirmInternalNavigation);
    }

    public event EventHandler<EventArgs> LocationChanged = default!;

    public event EventHandler<EventArgs> NeedRefresh = default!;

    public void RegisterLocationChangingHandler(Func<LocationChangingContext, ValueTask> handler)
    {
        navigationManager.RegisterLocationChangingHandler(handler);
    }

    private async ValueTask ConfirmInternalNavigation(LocationChangingContext locationChangingContext)
    {
        if (applicationDbContext?.ChangeTracker.HasChanges() == true)
        {
            bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Änderungen verwerfen?");
            if (!isConfirmed)
            {
                locationChangingContext.PreventNavigation();
            }
        }
    }

    private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        LocationChanged?.Invoke(this, e);
        if (applicationDbContext?.ChangeTracker.HasChanges() == true)
        {
            applicationDbContext.Discard();
            NeedRefresh?.Invoke(this, EventArgs.Empty);
        }
    }
}