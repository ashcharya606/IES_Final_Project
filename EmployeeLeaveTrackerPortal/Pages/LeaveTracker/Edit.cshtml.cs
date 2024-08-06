using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveTrackerPortal.Data;
using EmployeeLeaveTrackerPortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EmployeeLeaveTrackerPortal.Authorization;

namespace EmployeeLeaveTrackerPortal.Pages.LeaveTracker
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee? employee = await Context.Employee.FirstOrDefaultAsync(
                                                             m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            Employee = employee;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Employee,
                                                      EmployeeTrackerOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Contact from DB to get OwnerID.
            var employee = await Context
                .Employee.AsNoTracking()
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, employee,
                                                     EmployeeTrackerOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Employee.OwnerID = employee.OwnerID;

            Context.Attach(Employee).State = EntityState.Modified;

            if (Employee.Status == LeaveStatus.Approved)
            {
                // If the contact is updated after approval, 
                // and the user cannot approve,
                // set the status back to submitted so the update can be
                // checked and approved.
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Employee,
                                        EmployeeTrackerOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Employee.Status = LeaveStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
