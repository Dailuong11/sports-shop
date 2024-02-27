using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class EmpRepo : IEmpRepo
    {

        public async Task<Employee> Employee(int? id) => await EmpDAO.GetEmployeeById(id);

        public async Task<List<Employee>> Employees(PaginationParams @params, 
            string? name, 
            int? dep, 
            string? courtesy, 
            string? title,
            DateTime? from,
            DateTime? to) 
            => await EmpDAO.GetEmployees(@params, name, dep, courtesy, title, from, to);

        public async Task<List<Employee>> Employees(string? name, int? dep, string? courtesy, string? title, DateTime? from, DateTime? to) 
            => await EmpDAO.GetAll(name, dep, courtesy, title, from, to);

        public async Task<int> Save(Employee employee) => await EmpDAO.SaveEmployee(employee);

        public async Task<bool> Update(Employee employee) => await EmpDAO.UpdateEmployee(employee);

        public async Task<bool> Delete(Employee employee) => await EmpDAO.DeleteEmployee(employee);
    }
}
