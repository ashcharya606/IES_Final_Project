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
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IList<Employee>  Employee { get; set; }
        [BindProperty(SupportsGet = true)]
        public LeaveStatus? SelectedStatus { get; set; }
        public async Task OnGetAsync()
        {
            var contacts = from c in Context.Employee
                           select c;

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);


            // Only approved contacts are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                contacts = contacts.Where(c => c.Status == LeaveStatus.Approved
                                            || c.OwnerID == currentUserId);
            }
            // Apply filter if a status is selected
            if (SelectedStatus.HasValue)
            {
                contacts = contacts.Where(e => e.Status == SelectedStatus);
            }

            Employee = await contacts.ToListAsync();
        }
    }
}
