using BusinessObject.Models;
using BusinessObject.Req;
using DataAccess.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccess.DAO
{
	public class AccountDAO
	{
		private static readonly string Message = "Employee identity is null";
		public static async Task<List<Account>> GetCustomerAccounts()
		{
			var accounts = new List<Account>();
			using (var context = new PRN231DBContext())
			{
				accounts = await context.Accounts.Where(x => x.Role == 2).ToListAsync();
			}
			return accounts;
		}

		public static async Task<List<Account>> GetEmployeeAccounts()
		{
			var accounts = new List<Account>();
			using (var context = new PRN231DBContext())
			{
				accounts = await context.Accounts.Where(x => x.Role == 1).ToListAsync();
			}
			return accounts;
		}

		public static async Task<Account> AccountEmail(string? email)
		{
			Account? account;
			using (var context = new PRN231DBContext())
			{
				account = await context.Accounts.Include(x => x.Customer).SingleOrDefaultAsync(x => x.Email!.Equals(email));
			}
			return account ?? new();
		}
		public static async Task<Account> GetAccount(int? id)
		{
			Account? account;
			using (var context = new PRN231DBContext())
			{
				account = await context.Accounts.Where(a => a.AccountId == id).FirstOrDefaultAsync(); ;
			}
			return account ?? new();
		}

		public static async Task<Account> GetAccountToUpdate(string? email)
		{
			Account? account;
			using (var context = new PRN231DBContext())
			{
				account = await context.Accounts.Where(a => a.Email == email).FirstOrDefaultAsync(); ;
			}
			return account ?? new();
		}

		public static async Task<Account> GetAccount(AuthReq req)
		{
			Account? account;
			using (var context = new PRN231DBContext())
			{
				account = await context.Accounts
					.Where(a => a.Email!.Equals(req.Email)
				&& a.Password!.Equals(req.Password)).Include(x => x.Customer).Include(x => x.Employee).FirstOrDefaultAsync();
			}
			return account ?? new();
		}

        public static async Task<string?> GetAccount(string? email)
        {
            Account? account;
            using (var context = new PRN231DBContext())
            {
                account = await context.Accounts.Where(a => a.Email!.Equals(email)).FirstOrDefaultAsync();
                if (account is not null)
                {
                    var newPassword = RandomUtils.GenerateNewPassword(8);
                    account.Password = newPassword;
                    context.Entry<Account>(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                    return newPassword;
                }
                return null;
            }
        }

		public static async Task<bool> SaveCustomer(SignUpReq req)
		{
			req.customer!.CustomerId = RandomUtils.GenerateId(5);
			while (await CustomerDAO.GetCustomerById(req.customer.CustomerId) != null)
			{
				req.customer!.CustomerId = RandomUtils.GenerateId(5);
			}
			using (var context = new PRN231DBContext())
			{
				Account account = new Account
				{
					Email = req.Email,
					Password = req.Password,
					CustomerId = req.customer!.CustomerId,
					Customer = new Customer()
					{
						CustomerId = req.customer!.CustomerId,
						CompanyName = req.customer!.CompanyName ?? "Notyet",
						ContactName = req.customer!.ContactName,
						ContactTitle = req.customer!.ContactTitle,
						Address = req.customer!.Address
					},
					Role = req.Role
				};
				await context.Accounts.AddAsync(account);
				return await context.SaveChangesAsync() > 0;
			}
		}

		public static async Task<bool> SaveEmployee(EmployeeAccount employee)
		{
			var empId = await EmpDAO.SaveEmployee(GetEmployee(employee));
			if (empId == 0) throw new Exception(Message);
			using (var context = new PRN231DBContext())
			{
				Account account = new Account
				{
					Email = employee.Email,
					Password = employee.Password,
					EmployeeId = empId,
					Role = employee.Role
				};
				await context.Accounts.AddAsync(account);
				return await context.SaveChangesAsync() > 0;
			}
		}

		public static async Task<bool> SaveEmpAccount(EmpSignUpReq req)
		{
			using (var context = new PRN231DBContext())
			{
				var accountToUpdate = await GetAccountToUpdate(req.Email);
				if (accountToUpdate is not null)
				{
					accountToUpdate.Email = req.Email;
					accountToUpdate.Password = req.Password;
					context.Entry<Account>(accountToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
					return await context.SaveChangesAsync() > 0;
				}
				Account account = new Account
				{
					Email = req.Email,
					Password = req.Password,
					EmployeeId = req.EmployeeId,
					Role = req.Role
				};
				await context.Accounts.AddAsync(account);
				return await context.SaveChangesAsync() > 0;

			}
		}
		public static async Task<bool> UpdateAccount(Account account)
		{
			using (var context = new PRN231DBContext())
			{
				context.Entry<Account>(account).State
				= Microsoft.EntityFrameworkCore.EntityState.Modified;
				return await context.SaveChangesAsync() > 0;
			}
		}
		public static Employee GetEmployee(EmployeeAccount employee)
		{
			return new Employee
			{
				LastName = employee.LastName,
				FirstName = employee.FirstName,
				BirthDate = employee.BirthDate,
				Address = employee.Address,
				DepartmentId = employee.DepartmentId,
				HireDate = employee.HireDate,
				Title = employee.Title,
				TitleOfCourtesy = employee.TitleOfCourtesy
			};
		}

		public static Page PageView()
		{
			Page? page;
			using (var context = new PRN231DBContext())
			{
				page = context.Pages.FirstOrDefault();
			}
			return page;
		}
		public static async Task<bool> UpdatePage(Page page)
		{
			using (var context = new PRN231DBContext())
			{

				context.Entry<Page>(page).State
		  = Microsoft.EntityFrameworkCore.EntityState.Modified;
				return await context.SaveChangesAsync() > 0;

			}
		}
		public static async Task<Page> GetPageByID()
		{
			Page? page;
			using (var context = new PRN231DBContext())
			{
				page = await context.Pages.Where(x => x.Id == 1).FirstOrDefaultAsync();
			}
			return page ?? new();
		}
	}
}
