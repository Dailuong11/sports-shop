using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IDepRepo
    {
        Task<List<Department>> Departments(PaginationParams @params, string? name);
        Task<List<Department>> Departments(string? name);
        Task<Department> Department(int? id);
        Task<bool> Save(Department department);
        Task<bool> Update(Department department);
        Task<bool> Delete(Department department);
    }
}
