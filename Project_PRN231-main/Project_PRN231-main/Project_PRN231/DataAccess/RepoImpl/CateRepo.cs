using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class CateRepo : ICateRepo
    {
        public async Task<List<Category>> Categories(PaginationParams @params, string? name) => await CategoryDAO.GetCategories(@params, name);

        public async Task<List<Category>> Categories(string? name) => await CategoryDAO.GetCategories(name);

        public async Task<Category> Category(int? id) => await CategoryDAO.GetCategory(id);

        public async Task<bool> Save(Category category) => await CategoryDAO.SaveCategory(category);

        public async Task<bool> Delete(Category category) => await CategoryDAO.DeleteCategory(category);

        public async Task<bool> Update(Category category) => await CategoryDAO.UpdateCategory(category);

    }
}
