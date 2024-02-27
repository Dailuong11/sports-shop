using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IProductRepo
    {
      
        List<Product> GetTopProductsSale();
        List<Product> GetProductByCategoryID(int categoryid);
        List<Product> GetTop4Newest();
        Task<List<Product>> Products(PaginationParams @params, FilterParams @filter);
        Task<List<Product>> Products(FilterParams @filter);
        Task<Product> Product(int? id);
        Task<bool> Save(Product product);
        Task<bool> Save(List<Product> products);
        Task<bool> Update(Product product);
        Task<bool> Delete(Product product);
        List<string> GetAllProductName();

        Task<Product> GetProductByName(string? name);
    }
}
