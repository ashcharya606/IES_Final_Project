
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveTrackerPortal.Data;
using EmployeeLeaveTrackerPortal.Model;
using EmployeeLeaveTrackerPortal.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EmployeeLeaveTrackerPortal.Pages.LeaveTracker
{
    public class DetailsModel : DI_BasePageModel
    {
        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee? _employ = await Context.Employee.FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (_employ == null)
            {
                return NotFound();
            }
            Employee = _employ;

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Employee.OwnerID
                && Employee.Status != LeaveStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, LeaveStatus status)
        {
            var Employee = await Context.Employee.FirstOrDefaultAsync(
                                                      m => m.EmployeeId == id);

            if (Employee == null)
            {
                return NotFound();
            }

            var EmployeeOperation = (status == LeaveStatus.Approved)
                                                       ? EmployeeTrackerOperations.Approve
                                                       : EmployeeTrackerOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Employee,
                                        EmployeeOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            Employee.Status = status;
            Context.Employee.Update(Employee);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
