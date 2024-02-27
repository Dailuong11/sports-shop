using BusinessObject.Models;
using DataAccess.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CustomerDAO
    {
        public static async Task<List<Customer>> GetCustomers(PaginationParams @params, string? name, string? title)
        {
            var customers = await GetAll(name, title);
            return customers
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage)
                .ToList();
        }

        public static async Task<List<Customer>> GetAll(string? name, string? title)
        {
            var customers = new List<Customer>();
            using (var context = new PRN231DBContext())
            {
                customers = await context.Customers.Where(x => name == null || x.ContactName!.Contains(name)).Include(x => x.Accounts).ToListAsync();
                customers = customers.Where(x => title == null || x.ContactTitle!.Contains(title)).ToList();
            }
            return customers;
        }

        public static async Task<Customer?> GetCustomerById(string? id)
        {
            Customer? customer;
            using (var context = new PRN231DBContext())
            {
                customer = await context
                    .Customers.SingleOrDefaultAsync(x => x.CustomerId.Equals(id));
            }
            return customer ?? null;
        }
        
        public static async Task<string> SaveCustomer(Customer customer)
        {
            customer.CustomerId = RandomUtils.GenerateId(5);
            while (await GetCustomerById(customer.CustomerId) != null)
            {
                customer.CustomerId = RandomUtils.GenerateId(5);
            }
            using (var context = new PRN231DBContext())
            {
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
                return customer.CustomerId;
            }
        }

        public static async Task<bool> UpdateCustomer(Customer customer)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Customer>(customer).State
                = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return await context.SaveChangesAsync() > 0;
            }
        }

        public static async Task<bool> DeleteCustomer(Customer customer)
        {
            using (var context = new PRN231DBContext())
            {
                context.Entry<Customer>(customer).State
                = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return await context.SaveChangesAsync() > 0;
            }
        }
        public static int GetNumberOfCustomer()
        {
            int accounts;
            using (var context = new PRN231DBContext())
            {
                accounts = context.Customers.Distinct().Count();
            }
            return accounts;
        }

    }
}
