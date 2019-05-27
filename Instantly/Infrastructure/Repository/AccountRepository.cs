using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Instantly.Context;
using Instantly.Infrastructure.DbFactory;
using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Instantly.Infrastructure.Repository
{
    public class AccountRepository:RepositoryBase<Account,UserDbContext>,IAccountRepository
    {
        private readonly UserDbContext _userContext;
        private readonly InstagramControl instagramControl = InstagramControl.GetInstance();
        public AccountRepository(UserDbFactory dbFactory):base(dbFactory)
        {
            _userContext = dbFactory.Get();
        }

        public async Task AddAccount(IInstaApi api, Account account)
        {
            account.TextMessage = "Привет {UserName} спасибо что подписался";
            account.CountSendMessage = 0;
            account.IsActive = false;
            
            _userContext.Accounts.Add(account);
            await _userContext.SaveChangesAsync();

            await instagramControl.AddAndStartObservable(account.Login, api,account);  
        }

        public void StartObservable(string instagram)
        {
            var account = _userContext.Accounts.FirstOrDefault(p => p.Login == instagram);
            account.IsActive = true;
            _userContext.SaveChanges();
            instagramControl.StartObservable(instagram);
        }
        public void StopObservable(string instagram)
        {
            var account = _userContext.Accounts.FirstOrDefault(p => p.Login == instagram);
            account.IsActive = false;
            _userContext.SaveChanges();
            instagramControl.StopObservable(instagram);
        }
        public List<InstaCurrentUser> GetListAccounts(long userId)
        {
            var logins = _userContext.Accounts.Where(p => p.UserId == userId).Select(p=>p.Login).ToList();
            return instagramControl.GetListAccounts(logins);
        }

        public void UpdateText(Account account)
        {
            instagramControl.UpdateText(account);
            _userContext.SaveChanges();
        }
        public void DeleteAccount(Account account)
        {
            instagramControl.DeleteAccount(account);
        }

    }
    public interface IAccountRepository : IRepository<Account>
    {
        Task AddAccount(IInstaApi api, Account account);
        void StartObservable(string instagram);
        void StopObservable(string instagram);
        List<InstaCurrentUser> GetListAccounts(long userId);
        void UpdateText(Account account);
        void DeleteAccount(Account account);
    }
}