using BusinessObject.Models;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class OrderRepo : IOrderRepo
    {
        public async Task<Order> Order(int? id) => await OrderDAO.Order(id);

        public async Task<List<Order>> Orders(PaginationParams @params, DateTime? from, DateTime? to) => await OrderDAO.Orders(@params, from, to);

        public async Task<List<Order>> Orders(PaginationParams @params, DateTime? from, DateTime? to, string customerId) => await OrderDAO.Orders(@params, from, to, customerId);

        public async Task<List<Order>> Orders(DateTime? from, DateTime? to, string? customerId) => await OrderDAO.GetAll(from, to, customerId);

        public async Task<bool> Save(Order order) => await OrderDAO.Save(order);

        public async Task<bool> Update(Order order) => await OrderDAO.Update(order);

        public Task<bool> Delete(Order order) => OrderDAO.Delete(order);

        public async Task<List<Order>> OrderMonth() => await OrderDAO.getMonthOrder();
     //   public async Task<bool> Delete(Order order) => await OrderDAO.Delete(order);
    }
}
