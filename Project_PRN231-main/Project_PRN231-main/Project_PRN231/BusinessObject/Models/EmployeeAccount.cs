using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class EmployeeAccount
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; } = 1;
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public DateTime? HireDate { get; set; }
        public int? DepartmentId { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public int? EmployeeId { get; set; }
    }
}
