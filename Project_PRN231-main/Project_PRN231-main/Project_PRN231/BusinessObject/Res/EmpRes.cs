using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Res
{
    public class EmpRes
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int? DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public String? BirthDateString
        {
            get
            {
                return DateTime.Parse(BirthDate.ToString()!).ToString("dd/MM/yyyy");
            }
        }

        public DateTime? HireDate { get; set; }

        public String? HireDateString
        {
            get
            {
                return DateTime.Parse(HireDate.ToString()!).ToString("dd/MM/yyyy");
            }
        }

        public string? Address { get; set; }

    }
}
