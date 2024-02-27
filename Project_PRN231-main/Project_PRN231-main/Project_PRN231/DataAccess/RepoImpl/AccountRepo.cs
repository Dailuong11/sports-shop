using BusinessObject.Models;
using BusinessObject.Req;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepoImpl
{
    public class AccountRepo : IAccountRepo
    {
        public async Task<Account> Account(int? id) => await AccountDAO.GetAccount(id);

        public async Task<Account> Account(AuthReq req) => await AccountDAO.GetAccount(req);    

        public async Task<string?> Account(string? email) => await AccountDAO.GetAccount(email);

        public async Task<List<Account>> CustomersAccounts() => await AccountDAO.GetCustomerAccounts();

        public async Task<List<Account>> EmployeesAccounts() => await AccountDAO.GetEmployeeAccounts();

        public async Task<bool> Save(SignUpReq req) => await AccountDAO.SaveCustomer(req);

        public async Task<Account> AccountEmail(string ? email) => await AccountDAO.AccountEmail(email);

        public async Task<bool> Save(EmpSignUpReq req) => await AccountDAO.SaveEmpAccount(req);

        public async Task<bool> Save(EmployeeAccount employee) => await AccountDAO.SaveEmployee(employee);

        public async Task<bool> Update(Account account) => await AccountDAO.UpdateAccount(account);

        //public  Page PageView() =>  AccountDAO.PageView();

        //public async Task<Page> GetPageByID() => await AccountDAO.GetPageByID();
        //public async Task<bool> UpdatePage(Page page) => await AccountDAO.UpdatePage(page);

    }
}
