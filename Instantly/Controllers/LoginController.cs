using Instantly.Context;
using Instantly.Filtres;
using Instantly.Infrastructure.DbFactory;
using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Instantly.Controllers
{
    [RoutePrefix("api/Login")]
    public class LoginController: BaseController
    {
        
        private readonly UserDbContext _userContext;
        public LoginController(UserDbFactory userDbFactory)
        {
            _userContext = userDbFactory.Get();
        }

        [HttpGet]
        [Route("checkEmail")]
        public IHttpActionResult CheckEmail(string email)
        {
            
            return Ok(_userContext.Users.FirstOrDefault(p => p.Email == email) != null);
        }

        [HttpPost]
        [Route("Registry")]
        public async Task<IHttpActionResult> Registry(User user)
        {
            if(_userContext.Users.FirstOrDefault(p => p.Email == user.Email) != null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "mail is already in use"));
            }
            else
            {
                try
                {
                    user.IsFree = true;
                    user.DateEndSub = DateTime.Now.AddDays(3);
                    _userContext.Users.Add(user);
                    await _userContext.SaveChangesAsync();
                }
                catch(Exception ex)
                { 
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex?.InnerException.Message??ex.Message));
                }
                return Ok();
            }
        }


        [HttpGet]
        [Authorize]
        [Route("getData")]
        public IHttpActionResult GetDataFromToken()
        {
            try
            {
                var id=this.Request.Headers.GetUserId();
                var user = _userContext.Users.FirstOrDefault(p => p.Id == id);
                user.Password = "";
                return Ok(user);
            }
            catch(Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException?.Message??ex.Message));
            }


        }
    }
}