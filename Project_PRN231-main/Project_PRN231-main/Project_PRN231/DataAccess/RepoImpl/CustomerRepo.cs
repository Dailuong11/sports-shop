using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class CustomerRepo : ICustomerRepo
    {
        public async Task<Customer> Customer(string? id) => await CustomerDAO.GetCustomerById(id);

        public async Task<List<Customer>> Customers(PaginationParams @params, string? name, string? tile) => await CustomerDAO.GetCustomers(@params, name, tile);

        public async Task<List<Customer>> Customers(string? name, string ?title) => await CustomerDAO.GetAll(name, title);

        public async Task<bool> Delete(Customer customer) => await CustomerDAO.DeleteCustomer(customer);

        public async Task<string> Save(Customer customer) => await CustomerDAO.SaveCustomer(customer);

        public async Task<bool> Update(Customer customer) => await CustomerDAO.UpdateCustomer(customer);
    }
}
