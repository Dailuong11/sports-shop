using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDAO
    {
        public static async Task<List<Order>> Orders(PaginationParams @params, DateTime? from, DateTime? to)
        {
            var orders = await GetAll(from, to, null);
            return orders
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Order>> Orders(PaginationParams @params, DateTime? from, DateTime? to, string customerId)
        {
            var orders = await GetAll(from, to, customerId);
            return orders
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Order>> GetAll(DateTime? from, DateTime? to, string? customerId)
        {
            var orders = new List<Order>();
            using (var context = new PRN231DBContext())
            {
                orders = await context.Orders.Include(x => x.Customer)
                    .Include(x => x.Employee).ThenInclude(x => x!.Department)
                    .Include(x => x.OrderDetails).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
                    .ToListAsync();
                orders = customerId is null ? orders : orders.Where(x => x.CustomerId!.Equals(customerId)).ToList();
                orders = from is null ? orders : orders.Where(x => DateTime.Compare((DateTime)x.OrderDate!, (DateTime)from) > 0).ToList();
                orders = to is null ? orders : orders.Where(x => DateTime.Compare((DateTime)x.OrderDate!, (DateTime)to) < 0).ToList();
            }
            return orders;
        }

        public static async Task<Order> Order(int? orderId)
        {
            Order? order;
            using (var context = new PRN231DBContext())
            {
                order = await context
                    .Orders.Include(x => x.Customer)
                    .Include(x => x.Employee).ThenInclude(x => x!.Department)
                    .Include(x => x.OrderDetails).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
                    .SingleOrDefaultAsync(x => x.OrderId == orderId);
            }
            return order ?? new();
        }

        public static async Task<bool> Save(Order order)
        {
            using (var context = new PRN231DBContext())
            {
                order.OrderDetails.ToList().ForEach(x => x.Product = null);
                await context.Orders.AddAsync(order);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> Update(Order order)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Order>(order).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> Delete(Order order)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Order>(order).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
       
        public static async Task<List<Order>> getMonthOrder()
        {
            var orders = new List<Order>();
            using (var context = new PRN231DBContext())
            {
                orders = await context.Orders.Include(x => x.Customer)
                    .Include(x => x.Employee).ThenInclude(x => x!.Department)
                    .Include(x => x.OrderDetails).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
                    .ToListAsync();
                orders =  orders.Where(a => (a.OrderDate.HasValue && a.OrderDate.Value.Month == DateTime.Today.Month) && (a.OrderDate.HasValue && a.OrderDate.Value.Year == DateTime.Today.Year)).ToList();
            
            }
            return orders;
        }
    }
}
