using EmployeeLeaveTrackerPortal.Authorization;
using EmployeeLeaveTrackerPortal.Data;
using EmployeeLeaveTrackerPortal.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveTrackerPortal.Pages.LeaveTracker
{
    public class CreateModels : DI_BasePageModel
    {
        public CreateModels(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Employee.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Employee,
                                                        EmployeeTrackerOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Employee.Add(Employee);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
