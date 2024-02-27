using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CategoryDAO
    {
        public static async Task<List<Category>> GetCategories(PaginationParams @params, string? name)
        {
            var cates = await GetCategories(name);
            return cates
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }
        public static async Task<List<Category>> GetCategories(string? name)
        {
            List<Category> listCategories;
            using (var db = new PRN231DBContext())
            {
                listCategories = await db.Categories.ToListAsync();
                listCategories = listCategories
                    .Where(x => name == null || x.CategoryName.Equals(name)).ToList();
            }
            return listCategories;
        }

        public static async Task<Category> GetCategory(int? id)
        {
            Category? category;
            using (var context = new PRN231DBContext())
            {
                category = await context.Categories
                    .SingleOrDefaultAsync(x => x.CategoryId == id);
            }
            return category ?? new();
        }

        public static async Task<bool> SaveCategory(Category category)
        {
            using (var context = new PRN231DBContext())
            {
                await context.Categories.AddAsync(category);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> UpdateCategory(Category category)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Category>(category).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> DeleteCategory(Category category)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Category>(category).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}
