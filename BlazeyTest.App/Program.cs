using Blazey.Data.EntityFramework;
using Blazey.Data.Services;
using Blazey.Email;
using Blazey.Security.AdminCode;
using Blazey.Security.Services;
using BlazeyTest.Application.Components;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Security;
using BlazeyTest.Application.Services.DataServices.Data_Lists;
using BlazeyTest.Application.Services.DataServices.Data_User;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazeyTest.Application;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddScoped<NavigationService>();

        ConfigureServices(builder.Configuration, builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapControllers();
        app.MapRazorComponents<BlazeyTest.Application.Components.App>()
            .AddInteractiveServerRenderMode();

        app.UseStatusCodePagesWithRedirects("/error/{0}");

        app.Run();
    }

    private static void ConfigureAuthServices(IConfiguration Configuration, IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;

            options.Lockout.MaxFailedAccessAttempts = 6;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            // enables immediate logout, after updating the user's security stamp.
            options.ValidationInterval = TimeSpan.Zero;
        });

        services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        services.AddScoped<AuthorizationService<User>>();
    }

    private static void ConfigureCookieSerivce(IConfiguration Configuration, IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = "NiVo-Authentification";
            options.ExpireTimeSpan = TimeSpan.FromDays(14);

            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/error/404";
            options.SlidingExpiration = true;
        });
    }

    private static void ConfigureDataServices(IConfiguration Configuration, IServiceCollection services)
    {
        services.AddScoped<BaseDataHandler<User>, DH_User>();
        services.AddScoped<DS_User>();

        services.AddScoped<DH_RatedList>();
        services.AddScoped<DS_RatedList>();

        services.AddScoped<DH_RatedListEntry>();
        services.AddScoped<DS_RatedListEntry>();

        services.AddScoped<DH_RatedListEntryRating>();
        services.AddScoped<DS_RatedListEntryRating>();
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddControllers();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });

        ConfigureAuthServices(configuration, services);

        ConfigureCookieSerivce(configuration, services);

        ConfigureDataServices(configuration, services);

        services.AddScoped<NavigationService>();
        services.AddSingleton<AdminCodeService, AdminCodeService>((o) => new(configuration.GetValue<Guid>("AdminCode")));
        services.AddScoped<AuthorizationService<User>>();

        EmailSettings emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? throw new Exception("EmailSettings not found in appsettings.json");
        services.AddSingleton(emailSettings);
        services.AddTransient<EmailService>();
    }
}