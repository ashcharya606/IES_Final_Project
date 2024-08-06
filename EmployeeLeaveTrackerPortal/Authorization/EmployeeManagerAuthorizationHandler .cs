using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using EmployeeLeaveTrackerPortal.Model;

namespace EmployeeLeaveTrackerPortal.Authorization
{
    public class EmployeeManagerAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Employee>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Employee resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != EmployeeLeaveTrackerPortal.Authorization.Constants.ApproveOperationName &&
                requirement.Name != EmployeeLeaveTrackerPortal.Authorization.Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can approve or reject.
            if (context.User.IsInRole(EmployeeLeaveTrackerPortal.Authorization.Constants.ContactManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
