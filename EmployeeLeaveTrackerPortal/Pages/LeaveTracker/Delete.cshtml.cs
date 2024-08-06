using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveTrackerPortal.Data;
using EmployeeLeaveTrackerPortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EmployeeLeaveTrackerPortal.Authorization;

namespace EmployeeLeaveTrackerPortal.Pages.LeaveTracker
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(
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
            Employee? _employee = await Context.Employee.FirstOrDefaultAsync(
                                                 m => m.EmployeeId == id);

            if (_employee == null)
            {
                return NotFound();
            }
            Employee = _employee;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Employee,
                                                     EmployeeTrackerOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var employee = await Context
                .Employee.AsNoTracking()
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, employee,
                                                     EmployeeTrackerOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Employee.Remove(employee);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
    

