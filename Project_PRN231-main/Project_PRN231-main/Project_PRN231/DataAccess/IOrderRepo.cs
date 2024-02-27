using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IOrderRepo
    {
        Task<List<Order>> Orders(PaginationParams @params, DateTime? from, DateTime? to);
        Task<List<Order>> Orders(PaginationParams @params, DateTime? from, DateTime? to, string customerId);
        Task<List<Order>> Orders(DateTime? from, DateTime? to, string? customerId);
        Task<Order> Order(int? id);
        Task<bool> Save(Order order);
        Task<bool> Update(Order order);
        Task<bool> Delete(Order order);
        Task<List<Order>> OrderMonth();
    }
}
