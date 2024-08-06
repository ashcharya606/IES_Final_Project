using EmployeeLeaveTrackerPortal.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EmployeeLeaveTrackerPortal.Authorization;
namespace EmployeeLeaveTrackerPortal.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@tracker.com");
                await EnsureRole(serviceProvider, adminID, Constants.ContactAdministratorsRole);

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw, "manager@tracker.com");
                await EnsureRole(serviceProvider, managerID, Constants.ContactManagersRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Employee.Any())
            {
                return;   // DB has been seeded
            }

            context.Employee.AddRange(
               new Employee
               {

                   Name = "Debra Garcia",
                   Department = "Testing",
                   Date = "12/08/2024",
                   Reason = "Housewarming",
                   Email = "debra@example.com",
                   Status = LeaveStatus.Approved,
                   OwnerID = adminID
               },
         new Employee
         {

             Name = "Thorsten Weinrich",
             Department = "Financial",
             Date = "19/08/2024",
             Reason = "Family Function",
             Email = "thorsten@example.com",
             Status = LeaveStatus.Submitted,
             OwnerID = adminID
         },
         new Employee
         {

             Name = "Yuhong Li",
             Department = "Testing",
             Date = "20/08/2024",
             Reason = "Personal Reason",
             Email = "yuhong@example.com",
             Status = LeaveStatus.Rejected,
             OwnerID = adminID
         },
          new Employee
          {

              Name = "Chandana Prasad",
              Department = "Develper",
              Date = "12/08/2024",
              Reason = "Personal Reason",
              Email = "cp@gmail.com",
              Status = LeaveStatus.Submitted,
              OwnerID = adminID



          },
         new Employee
         {

             Name = "Shelson Thomas",
             Department = "Testing",
             Date = "12/08/2024",
             Reason = "Vacation",
             Email = "shekson@gmail.com",
             Status = LeaveStatus.Approved,
             OwnerID = adminID

         },
         new Employee
         {

             Name = "Sharon Reji",
             Department = "HR",
             Date = "09/09/2024",
             Reason = "Off",
             Email = "sharon@gmail.com",
             Status = LeaveStatus.Submitted,
             OwnerID = adminID

         },
         new Employee
         {

             Name = "Anu Ms",
             Department = "UI",
             Date = "17/08/2024",
             Reason = "Holiday",
             Email = "anu@gmail.com",
             Status = LeaveStatus.Submitted,
             OwnerID = adminID

         }
      );

            context.SaveChanges();
        }


    }
}
