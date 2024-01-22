using Blazey.Email;
using Blazey.Security.AdminCode;
using Blazey.Security.Controllers;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BlazeyTest.Application.Components.Pages.Account;

[Microsoft.AspNetCore.Mvc.Route("/account/[controller]")]
[Controller]
public class APIController : AccountAPIController<User>
{
    private readonly AdminCodeService adminCodeService;

    private readonly EmailService emailService;

    private readonly SignInManager<User> signInManager;

    private readonly UserManager<User> userManager;

    public APIController(SignInManager<User> signInManager, UserManager<User> userManager, AdminCodeService adminCodeService, EmailService emailService) : base(userManager)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.adminCodeService = adminCodeService;
        this.emailService = emailService;
    }

    public static string TranslateIdentityError(IdentityError? error)
    {
        if (error == null)
        {
            return "Ein unbekannter Fehler ist aufgetreten.";
        }

        switch (error.Code)
        {
            case "DefaultError":
                return "Ein nicht spezifizierter Fehler ist aufgetreten.";

            case "ConcurrencyFailure":
                return "Ein Konflikt bei gleichzeitigem Zugriff ist aufgetreten. Bitte versuchen Sie es erneut.";

            case "PasswordMismatch":
                return "Das eingegebene Passwort ist falsch.";

            case "InvalidToken":
                return "Das Token ist ungültig oder abgelaufen.";

            case "LoginAlreadyAssociated":
                return "Es gibt bereits einen Benutzer mit diesem Login.";

            case "InvalidUserName":
                return $"Der Benutzername ist ungültig. Er muss einen Wert enthalten und darf keine ungültigen Zeichen enthalten.";

            case "InvalidEmail":
                return $"Die E-Mail-Adresse ist ungültig.";

            case "DuplicateUserName":
                return $"Die E-Mail-Adresse wird bereits verwendet.";

            case "DuplicateEmail":
                return $"Die E-Mail-Adresse wird bereits verwendet.";

            case "ExternalLoginExists":
                return "Es gibt bereits einen Benutzer mit diesem externen Login.";

            case "UserAlreadyHasPassword":
                return "Der Benutzer hat bereits ein Passwort.";

            case "UserLockoutNotEnabled":
                return "Die Kontosperre ist nicht aktiviert.";

            case "PasswordTooShort":
                return "Das Passwort ist zu kurz. Es muss mindestens 8 Zeichen lang sein.";

            case "PasswordRequiresNonAlphanumeric":
                return "Das Passwort muss mindestens ein Sonderzeichen enthalten.";

            case "PasswordRequiresDigit":
                return "Das Passwort muss mindestens eine Ziffer enthalten.";

            case "PasswordRequiresLower":
                return "Das Passwort muss mindestens einen Kleinbuchstaben enthalten.";

            case "PasswordRequiresUpper":
                return "Das Passwort muss mindestens einen Großbuchstaben enthalten.";

            case "UserNotAllowed":
                return "Der Benutzer ist nicht berechtigt.";

            case "UserNotInRole":
                return "Der Benutzer gehört nicht zur angegebenen Rolle.";

            case "UserAlreadyInRole":
                return "Der Benutzer gehört bereits zur angegebenen Rolle.";

            case "RoleNotFound":
                return "Die angegebene Rolle wurde nicht gefunden.";

            case "RoleExists":
                return "Die angegebene Rolle existiert bereits.";

            case "InvalidRoleName":
                return "Der Rollenname ist ungültig.";

            case "RecoveryCodeRedemptionFailed":
                return "Die Wiederherstellungs-Codes konnten nicht eingelöst werden.";

            case "LockoutNotEnabled":
                return "Die Kontosperre ist für den Benutzer nicht aktiviert.";

            case "MaxTwoFactorLoginAttempts":
                return "Die maximale Anzahl von Zwei-Faktor-Authentifizierungsversuchen wurde überschritten.";
            // Fügen Sie hier weitere Fehlercodes hinzu, die Sie behandeln möchten.
            default:
                return "Ein unbekannter Fehler ist aufgetreten."; // Standardfall für nicht spezifizierte Fehler
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> LoginAsync(string username, string password, bool rememberMe, string returnUrl)
    {
        string Message = "";

        try
        {
            if (username.Length < 1 || password.Length < 1)
            {
                return Redirect(returnUrl, false, "Die Anmeldung ist fehlgeschlagen.");
            }

            var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, false);

            if (!result.Succeeded)
            {
                return Redirect(returnUrl, false, "Die Anmeldung ist fehlgeschlagen.");
            }
        }
        catch
        {
            return Redirect(returnUrl, false, "Anmeldung fehlgeschlagen: unbekannter Fehler.");
        }

        return Redirect(returnUrl == "/account/login" ? "/" : returnUrl, true);
    }

    [HttpPost("[action]")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync(string? ReturnUrl)
    {
        await signInManager.SignOutAsync();
        return Redirect(ReturnUrl ?? "~/");
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RegisterAdmin(string AdminCode, string Username, string Password, string ReturnUrl)
    {
        if (adminCodeService.Validate(AdminCode))
        {
            User User = new() { UserName = Username, Email = Username + "@mail.com", EmailConfirmed = true };
            User.PermissionSet.IsSysAdmin = true;
            if (await DoRegisterAsync(User, Password) is IdentityResult result)
            {
                return Redirect(ReturnUrl);
            }
        }
        return new ForbidResult();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RegisterAsync(string Email, string Firstname, string Lastname, string Password, string ReturnUrl)
    {
        User User = new() { UserName = Email, Email = Email, Firstname = Firstname, Lastname = Lastname };
        if (await DoRegisterAsync(User, Password) is IdentityResult result)
        {
            if (result.Succeeded)
            {
                if (await userManager.FindByNameAsync(Email) is User user)
                {
                    string token = Convert.ToBase64String(Encoding.ASCII.GetBytes(await userManager.GenerateEmailConfirmationTokenAsync(user)));
                    try
                    {
                        await emailService.SendEmailAsync(user.Email ?? string.Empty, "Email-Adresse für GameIT bestätigen", $"Hallo {user.Firstname},<br><br>Klicken Sie auf den folgenden Link, um Ihre Email-Adresse zu bestätigen:<br><br>{$"<a href=\"{Request.Scheme}://{Request.Host.Value}/account/confirm/{user.Id.ToString("N")}/{token}\">Hier klicken um Email zu Bestätigen</a>"}<br><br>Vielen Dank!<br><br>Das GameIT-Team");
                        return Redirect("/account/register/success");
                    }
                    catch
                    {
                        return Redirect(ReturnUrl, false, "Ein Bestätigungslink konnt nicht an Ihre Email gesendet werden.");
                    }
                }
                return Redirect(ReturnUrl, false, "Die Registrierung ist fehlgeschlagen.");
            }
            else
            {
                return Redirect(ReturnUrl, result.Errors.FirstOrDefault());
            }
        }
        return Redirect(ReturnUrl, false, "Die Registrierung ist aufgrund eines internen Fehlers fehlgeschlagen.");
    }

    private IActionResult Redirect(string ReturnUrl, bool Success, string? Message = null)
    {
        if (Message == null)
        {
            return Redirect(ReturnUrl);
        }
        return Redirect($"{ReturnUrl}?Success={Success.ToString()}&Base64Message={Convert.ToBase64String(Encoding.ASCII.GetBytes(Message))}");
    }

    private IActionResult Redirect(string ReturnUrl, IdentityError? Error)
    {
        return Redirect(ReturnUrl, false, TranslateIdentityError(Error));
    }
}