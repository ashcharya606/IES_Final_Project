using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveTrackerPortal.Model
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        // user ID from AspNetUser table.
        public string? OwnerID { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Department is required")]
        public string? Department { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public string? Date { get; set; }
        public string? Reason { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public LeaveStatus Status { get; set; }
    }
    public enum LeaveStatus
    {
        Submitted,
        Approved,
        Rejected
    }

}
