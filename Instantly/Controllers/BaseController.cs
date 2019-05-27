﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Instantly.Controllers
{
    public class BaseController : ApiController
    {
        public IHttpActionResult SendResponse<T>(T response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (statusCode != HttpStatusCode.OK)
            {
                // leave it up to microsoft to make this way more complicated than it needs to be
                // seriously i used to be able to just set the status and leave it at that but nooo... now 
                // i need to throw an exception 
                var badResponse =
                    new HttpResponseMessage(statusCode)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
                    };

                throw new HttpResponseException(badResponse);
            }
            return Ok(response);
        }
    }
}