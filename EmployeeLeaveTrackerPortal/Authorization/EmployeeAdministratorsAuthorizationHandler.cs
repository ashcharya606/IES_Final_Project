using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using EmployeeLeaveTrackerPortal.Model;

namespace EmployeeLeaveTrackerPortal.Authorization
{
    public class EmployeeAdministratorsAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Employee>
    {
        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement,
                                     Employee resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(EmployeeLeaveTrackerPortal.Authorization.Constants.ContactAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
