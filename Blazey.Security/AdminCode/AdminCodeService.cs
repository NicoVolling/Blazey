namespace Blazey.Security.AdminCode;

public class AdminCodeService
{
    public AdminCodeService(Guid AdminCode)
    {
        this.AdminCode = AdminCode;
    }

    public Guid AdminCode { get; init; }

    public bool Validate(string AdminCode)
    {
        if (Guid.TryParse(AdminCode, out Guid guid))
        {
            return this.AdminCode == guid;
        }
        return false;
    }
}