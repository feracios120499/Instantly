using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using Instantly.Context;
using Instantly.Helpers;
using Instantly.Infrastructure;
using Instantly.Infrastructure.DbFactory;
using Instantly.Infrastructure.Repository;
using Instantly.Models;
using Instantly.ParametersModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Instantly.Controllers
{
    public static class RequestHeadersExtension
    {
        public static bool CheckUserId(this HttpRequestHeaders header, long userId)
        {
            IEnumerable<string> values;
            header.TryGetValues("Authorization", out values);
            var token = values.FirstOrDefault();
            var id = TokenHelper.GetUserIdFromToken(token);
            return id == userId;
        }
        public static long GetUserId(this HttpRequestHeaders header)
        {
            IEnumerable<string> values;
            header.TryGetValues("Authorization", out values);
            var token = values.FirstOrDefault();
            var id = TokenHelper.GetUserIdFromToken(token);
            return id;
        }
    }

    [Authorize]
    [RoutePrefix("api/accounts")]
    public class AccountController:BaseController
    {
        private static IDictionary<Account, IInstaApi> listApi=new Dictionary<Account, IInstaApi>();
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostAccount(InstagramAccount instagramAccount)
        {
            if (!this.Request.Headers.CheckUserId(instagramAccount.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            if (_accountRepository.Get(p=>p.Login== instagramAccount.Login) != null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "This account is already attached to another email"));
            }           
            else
            {
                var account = new Account(instagramAccount);
                var userSession = new UserSessionData
                {
                    UserName = account.Login,
                    Password = account.Password
                };
                string path = HttpContext.Current.Server.MapPath("~/App_Data/")+ userSession.UserName + ".bin";
                var InstaApi = InstaApiHelper.GetInstaApi(userSession,path);
                //Load session
                InstaApiHelper.LoadSession(InstaApi);
                if (!InstaApi.IsUserAuthenticated)
                {
                    var loginRequest = await InstaApi.LoginAsync();
                    if (loginRequest.Succeeded)
                    {
                        InstaApiHelper.SaveSession(InstaApi);
                        await _accountRepository.AddAccount(InstaApi, account);
                        return Ok();

                    }
                    listApi.Add(account, InstaApi);
                    return Ok(loginRequest);
                }
                else
                {
                    await _accountRepository.AddAccount(InstaApi, account);
                    return Ok();
                }
            }
        }

        [HttpPost]
        [Route("challenge/email/{login}")]
        public async Task<IHttpActionResult> SendCodeToEmail(string login)
        {
            if (!listApi.Any(p => p.Key.Login == login))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            var data = listApi.FirstOrDefault(p => p.Key.Login == login);
            var api = data.Value;
            var account = data.Key;
            if (!this.Request.Headers.CheckUserId(account.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            var email = await api.RequestVerifyCodeToEmailForChallengeRequireAsync();
            if (email.Succeeded)
            {
                return Ok(email);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, email.Info.Message));
            }
               
        }

        [HttpPost]
        [Route("challenge/phone/{login}")]
        public async Task<IHttpActionResult> SendCodeToPhone(string login)
        {
            if (!listApi.Any(p => p.Key.Login == login))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            var data = listApi.FirstOrDefault(p => p.Key.Login == login);
            var api = data.Value;
            var account = data.Key;
            if (!this.Request.Headers.CheckUserId(account.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            var phone = await api.RequestVerifyCodeToSMSForChallengeRequireAsync();
            if (phone.Succeeded)
            {
                return Ok(phone);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, phone.Info.Message));
            }


        }

        [HttpPost]
        [Route("challenge/verify")]
        public async Task<IHttpActionResult> VerifyCode(TwoFactor twoFactor)
        {
            if (!listApi.Any(p => p.Key.Login == twoFactor.Login))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            var data = listApi.FirstOrDefault(p => p.Key.Login == twoFactor.Login);
            var api = data.Value;
            var account = data.Key;
            if (!this.Request.Headers.CheckUserId(account.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            var verifyLogin = await api.VerifyCodeForChallengeRequireAsync(twoFactor.Code);
            if (verifyLogin.Succeeded)
            {
                return Ok(verifyLogin);
            }
            else
            {    
                // two factor is required
                if (verifyLogin.Value == InstaLoginResult.TwoFactorRequired)
                {
                    return Ok(verifyLogin);
                }
                else
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, verifyLogin.Info.Message));
                }      
            }
        }

        [HttpPost]
        [Route("twoFactor/verify")]
        public async Task<IHttpActionResult> VerifyTwoFactor(TwoFactor data)
        {
            if (!listApi.Any(p => p.Key.Login == data.Login))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            else
            {
                var api = listApi.FirstOrDefault(p => p.Key.Login == data.Login).Value;
                var account = listApi.FirstOrDefault(p => p.Key.Login == data.Login).Key;
                var twoFactorLogin = await api.TwoFactorLoginAsync(data.Code);
                if (!this.Request.Headers.CheckUserId(account.UserId))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
                }
                if (twoFactorLogin.Succeeded)
                {
                    InstaApiHelper.SaveSession(api);
                    await _accountRepository.AddAccount(api, account);
                    return Ok();
                }
                else
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, twoFactorLogin.Info.Message));
                }
            }
        }

        [HttpPut]
        [Route("autoreply/start/{login}")]
        public async Task<IHttpActionResult> StartObservable(string login)
        {
            var account = _accountRepository.Get(p => p.Login == login);

            if (account == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            if (!this.Request.Headers.CheckUserId(account.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            if (account.IsActive)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Auto reply is already on"));
            }
            try
            {
                _accountRepository.StartObservable(login);
                return Ok();
            }
            catch(Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException?.Message??ex.Message));
            }
        }

        [HttpPut]
        [Route("autoreply/stop/{login}")]
        public async Task<IHttpActionResult> StopObservable(string login)
        {
            var account = _accountRepository.Get(p => p.Login == login);
            if (account == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            if (!this.Request.Headers.CheckUserId(account.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            if (!account.IsActive)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Auto reply is already off"));
            }
            try
            {
                _accountRepository.StopObservable(login);
                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException?.Message ?? ex.Message));
            }
        }
        [HttpPut]
        [Route("autoreply/text")]
        public async Task<IHttpActionResult> UpdateText(UpdateText updateText)
        {
            var account = _accountRepository.Get(p => p.Login == updateText.Login);
            if (account == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            if (!this.Request.Headers.CheckUserId(account.UserId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            account.TextMessage = updateText.TextMessage;
            _accountRepository.UpdateText(account);
            return Ok();
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> RemoveAccount(string login)
        {
            var account=_accountRepository.Get(p => p.Login == login);
            if (account != null)
            {
                if (!this.Request.Headers.CheckUserId(account.UserId))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
                }
                _accountRepository.Delete(p => p.Login == account.Login);
                await _accountRepository.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Not Found Account"));
            }
            
            
        }


        [HttpGet]
        [Route("{userId}")]
        public async Task<IHttpActionResult> GetAccounts(long userId)
        {
            if (!this.Request.Headers.CheckUserId(userId))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Access is denied"));
            }
            try
            {

                var accounts = _accountRepository.GetListAccounts(userId);
                return Ok(accounts);
            }
            catch(Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException?.Message??ex.Message));
            }
        }
        [HttpPost]
        [Route("start")]
        public async Task Start()
        {

        }
    }
}