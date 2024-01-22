namespace BlazeyTest.Application.Data.Security;

public class UserInput
{
    public string? CurrentPassword { get; set; }

    public string? NewPassword { get; set; }

    public bool ResetPassword { get => NewPassword != null; set => NewPassword = value ? "" : null; }
}
