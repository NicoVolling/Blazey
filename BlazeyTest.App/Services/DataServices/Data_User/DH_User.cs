using Blazey.Data;
using Blazey.Data.Services;
using BlazeyTest.Application.Components.Pages.Account;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazeyTest.Application.Services.DataServices.Data_User;

public class DH_User : BaseDataHandler<User>
{

    public DH_User(ApplicationDbContext DbContext, UserManager<User> userManager) : base(DbContext)
    {
        UserManager = userManager;
    }

    public UserManager<User> UserManager { get; }

    public override void Create(User NewObject)
    {
        if (UserManager.CreateAsync(NewObject, NewObject.InputDataObject.NewPassword ?? string.Empty).Result is IdentityResult Result)
        {
            if (!Result.Succeeded)
            {
                string ErrorMessage = TaskResultMessageCollection.A_unexpected_error_occured();
                throw new Exception(ErrorMessage);
            }
            else
            {
                UpdateSubObjects(NewObject);
            }
        }
    }

    public override IQueryable<User> Get(bool ReadOnly = false)
    {
        return base.Get(ReadOnly).Include(o => o.Permissions);
    }

    protected override void OnChange(User ExistingObject, User NewObject)
    {
        if (NewObject.InputDataObject.NewPassword != null && NewObject.InputDataObject.CurrentPassword != null)
        {
            IdentityResult passwordChangeResult = UserManager.ChangePasswordAsync(ExistingObject, NewObject.InputDataObject.CurrentPassword, NewObject.InputDataObject.NewPassword).Result;

            if (!passwordChangeResult.Succeeded)
            {
                string ErrorMessage = APIController.TranslateIdentityError(passwordChangeResult.Errors.FirstOrDefault());
                throw new Exception(ErrorMessage);
            }

            if (TryGetById(ExistingObject.Id, out User? UpdatedExistingObject))
            {
                NewObject.PasswordHash = UpdatedExistingObject?.PasswordHash;
            }
        }
        base.OnChange(ExistingObject, NewObject);
        UpdateSubObjects(NewObject);
    }

    protected void UpdateSubObjects(User Object)
    {
        //if (DH_Human.TryGetById(Object.Human.Id, out Human? Human) && Human != null)
        //{
        //    DH_Human.Change(Object.Human.Id, Object.Human);
        //}
        //else
        //{
        //    DH_Human.Create(Object.Human);
        //}
    }
}
