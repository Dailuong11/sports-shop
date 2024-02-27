using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ProductDAO
    {
        public static List<Product> GetTopProductsSale()
        {
            var bestSaleProducts = new List<Product>();
            using (var context = new PRN231DBContext())
            {
                var listOrderDetails = context.OrderDetails
                            .Select(x => x.ProductId)
                            .Distinct()
                            .ToList();
                var listMostOrderProducts = listOrderDetails
                    .Select(id =>
                    {
                        int count = context.OrderDetails
                                    .Where(x => x.ProductId == id)
                                     .Count();
                        return new
                        {
                            ProductId = id,
                            Count = count
                        };
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
                var listBestSaleProdcutsId = listMostOrderProducts
                                           .Take(3)
                                           .Select(x => x.ProductId)
                                           .ToHashSet();
                bestSaleProducts = context.Products
                                    .Include(x => x.Category)
                                    .Where(x => listBestSaleProdcutsId
                                                 .Contains(x.ProductId))
                                    .ToList();
            }
            return bestSaleProducts;
        }
        public static List<Product> GetTop4Newest()
        {
            var gettop4 = new List<Product>();
            using (var context = new PRN231DBContext())
            {
                gettop4 = context.Products.Include(x => x.Category).OrderByDescending(x => x.ProductId).Take(3).ToList();
            }
            return gettop4;
        }

        public static List<Product> GetProductByCategoryID(int id)
        {
            var gettop4byCate = new List<Product>();
            using (var context = new PRN231DBContext())
            {
                gettop4byCate = context.Products.Include(x => x.Category).OrderByDescending(x => x.ProductId).Where(x => x.CategoryId == id).Take(3).ToList();
            }
            return gettop4byCate;
        }
        public static async Task<List<Product>> GetProducts(PaginationParams @params, FilterParams @filter)
        {
            var products = await GetAll(@filter);
            return products
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Product>> GetAll(FilterParams @filter)
        {
            var products = new List<Product>();
            using (var context = new PRN231DBContext())
            {
                products = await context.Products.Include(x => x.Category).ToListAsync();

                products = string.IsNullOrEmpty(@filter.productName) ? products :
                    products.Where(x => x.ProductName.ToLower().Contains(@filter.productName.ToLower())).ToList();

                products = @filter.categoryId is null || filter.categoryId == 0 ? products :
                    products.Where(x => x.CategoryId == @filter.categoryId).ToList();

                products = @filter.UnitPrice is null ? products :
                    products.Where(x => x.UnitPrice == @filter.UnitPrice).ToList();

            }
            return products;
        }

        public static async Task<Product> GetProductById(int? id)
        {
            Product? product;
            using (var context = new PRN231DBContext())
            {
                product = await context
                    .Products.Include(x => x.Category)
                    .SingleOrDefaultAsync(x => x.ProductId == id);
            }
            return product ?? new();
        }

        public static async Task<bool> SaveProduct(Product product)
        {
            using (var context = new PRN231DBContext())
            {
                await context.Products.AddAsync(product);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> SaveProducts(List<Product> products)
        {
            using (var context = new PRN231DBContext())
            {
                await context.Products.AddRangeAsync(products);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> UpdateProduct(Product product)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Product>(product).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> DeleteProduct(Product product)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Product>(product).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
        public static List<string> GetAllProductName()
        {
            var getAllName = new List<string>();
            using (var context = new PRN231DBContext())
            {
                getAllName = context.Products.Select(c => c.ProductName).ToList();
            }
            return getAllName;
        }
        public static async Task<Product> GetProductByName(string? name)
        {
            Product? product;
            using (var context = new PRN231DBContext())
            {
                product = await context
                    .Products.Include(x => x.Category)
                    .SingleOrDefaultAsync(x => x.ProductName.Equals(name));
            }
            return product ?? new();
        }
    }
}
