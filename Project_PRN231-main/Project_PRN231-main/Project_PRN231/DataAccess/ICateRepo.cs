using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface ICateRepo
    {
        Task<List<Category>> Categories(string? name);
        Task<List<Category>> Categories(PaginationParams @params, string? name);
        Task<Category> Category(int? id);
        Task<bool> Save(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(Category category);

    }
}
