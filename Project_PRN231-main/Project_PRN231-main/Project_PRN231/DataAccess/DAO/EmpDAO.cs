using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class EmpDAO
    {
        public static async Task<List<Employee>> GetEmployees(PaginationParams @params, 
            string? name, 
            int? dep, 
            string? courtesy, 
            string? title,
            DateTime? from,
            DateTime? to)
        {
            var emps = await GetAll(name, dep, courtesy, title, from, to);
            return emps
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Employee>> GetAll(string? name, 
            int? dep, 
            string? courtesy, 
            string? title,
            DateTime? from,
            DateTime? to)
        {
            var employees = new List<Employee>();
            using (var context = new PRN231DBContext())
            {
                employees = await context.Employees.Include(x => x.Department).Include(x => x.Accounts).ToListAsync();
                employees = dep is null ? employees : employees.Where(x => x.DepartmentId == dep).ToList();
                employees = string.IsNullOrEmpty(name) ? employees : employees.Where(x => x.FullName.ToLower().Contains(name.ToLower())).ToList();
                employees = string.IsNullOrEmpty(courtesy) ? employees : employees.Where(x => x.TitleOfCourtesy!.Contains(courtesy)).ToList();
                employees = string.IsNullOrEmpty(title) ? employees : employees.Where(x => x.Title!.Contains(title)).ToList();
                employees = from is null ? employees : employees.Where(x => x.HireDate >= from).ToList();
                employees = to is null ? employees : employees.Where(x => x.HireDate <= to).ToList();
            }
            return employees;
        }

        public static async Task<Employee> GetEmployeeById(int? id)
        {
            Employee? employee;
            using (var context = new PRN231DBContext())
            {
                employee = await context
                    .Employees.Include(x => x.Department).Include(x => x.Accounts)
                    .SingleOrDefaultAsync(x => x.EmployeeId == id);
            }
            return employee ?? new();
        }

        public static async Task<int> SaveEmployee(Employee employee)
        {
            using (var context = new PRN231DBContext())
            {
                await context.Employees.AddAsync(employee);
                await context.SaveChangesAsync();
                return employee.EmployeeId;
            }
        }

        public static async Task<bool> UpdateEmployee(Employee employee)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Employee>(employee).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> DeleteEmployee(Employee employee)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Employee>(employee).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}