using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class DepRepo : IDepRepo
    {
        public async Task<List<Department>> Departments(PaginationParams @params, string? name) => await DepartDAO.GetDepartments(@params, name);

        public async Task<List<Department>> Departments(string? name) => await DepartDAO.GetAll(name);

        public async Task<Department> Department(int? id) => await DepartDAO.GetDepartmentById(id);

        public async Task<bool> Save(Department department) => await DepartDAO.SaveDep(department);

        public async Task<bool> Update(Department department) => await DepartDAO.UpdateDep(department);

        public async Task<bool> Delete(Department department) => await DepartDAO.DeleteDep(department);
    }
}
