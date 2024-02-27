using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class DepartDAO
    {
        public static async Task<List<Department>> GetDepartments(PaginationParams @params, string? name)
        {
            var deps = await GetAll(name);
            return deps
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Department>> GetAll(string? name)
        {
            var deps = new List<Department>();
            using (var context = new PRN231DBContext())
            {
                deps = await context.Departments.Where(x => name == null || x.DepartmentName!.Contains(name)).ToListAsync();
            }
            return deps;
        }

        public static async Task<Department> GetDepartmentById(int? id)
        {
            Department? department;
            using (var context = new PRN231DBContext())
            {
                department = await context
                    .Departments
                    .SingleOrDefaultAsync(x => x.DepartmentId == id);   
            }
            return department ?? new();
        }

        public static async Task<bool> SaveDep(Department department)
        {
            using (var context = new PRN231DBContext())
            {
                await context.Departments.AddAsync(department);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> UpdateDep(Department department)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Department>(department).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> DeleteDep(Department department)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Department>(department).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}
