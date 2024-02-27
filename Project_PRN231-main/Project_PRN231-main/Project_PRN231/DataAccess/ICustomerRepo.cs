using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface ICustomerRepo
    {
        Task<List<Customer>> Customers(PaginationParams @params, string? name, string? tile);
        Task<List<Customer>> Customers(string? name, string? title);
        Task<Customer> Customer(string? id);
        Task<string> Save(Customer customer);
        Task<bool> Update(Customer customer);
        Task<bool> Delete(Customer customer);
    }
}
