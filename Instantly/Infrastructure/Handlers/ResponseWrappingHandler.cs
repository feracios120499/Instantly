using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Instantly.Infrastructure.Handlers
{
    public class ResponseWrappingHandler : DelegatingHandler
    {
        private bool ResponseIsValid(HttpResponseMessage response)
        {
            if (!(response != null && response.Content is ObjectContent)) return false;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
              task =>
              {
                  bool isResponseValid = task.Result.Content == null || ResponseIsValid(task.Result);
                  if (isResponseValid && !request.RequestUri.AbsoluteUri.Contains("swagger") && !request.RequestUri.AbsoluteUri.Contains("bankid"))
                  {
                      object responseObject;
                      task.Result.TryGetContentValue(out responseObject);
                      var useWrapper = true;
                      if (!request.Headers.Contains("Use-Response-Wrapper")
                        || request.Headers.GetValues("Use-Response-Wrapper").FirstOrDefault()?.ToLower() == "true")
                      {
                          ProcessObject(responseObject as IQueryable<object>, task.Result, false, useWrapper);
                      }

                  }
                  return task.Result;
              }, cancellationToken);
        }

        private void ProcessObject<T>(IEnumerable<T> responseObject, HttpResponseMessage response, bool isIQueryable, bool useWrapper) where T : class
        {
            var metadata = new Metadata<T>(response, isIQueryable);
            IEnumerable<string> originalSize;

            response.Headers.TryGetValues("originalSize", out originalSize);
            response.Headers.Remove("originalSize");
            if (originalSize != null)
            {
                metadata.TotalResults = Convert.ToInt32(originalSize.FirstOrDefault());
            }

            //uncomment this to preserve content negotation, but remember about typecasting for DataContractSerliaizer
            //var formatter = GlobalConfiguration.Configuration.Formatters.First(t => t.SupportedMediaTypes.Contains(new MediaTypeHeaderValue(response.Content.Headers.ContentType.MediaType)));
            //response.Content = new ObjectContent<Metadata<T>>(metadata, formatter);

            // ! завжди ввадаемо виклик API успішним (200), код реальної відповіді всередині 
            //if (useWrapper)
            //{
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ObjectContent<Metadata<T>>(metadata,
                GlobalConfiguration.Configuration.Formatters[0]);
            //}

        }
    }
}