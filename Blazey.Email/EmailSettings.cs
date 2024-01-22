namespace Blazey.Email;

public class EmailSettings
{
    public int MailPort { get; set; }

    public string MailServer { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}
