using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.OData;
using System.Web.OData.Query;

namespace Instantly.Filtres
{
    public class ODataQueryableAttribute : EnableQueryAttribute
    {
        private int? _queryCount;
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            object responseObject;

            base.OnActionExecuted(actionExecutedContext);

            if (ResponseIsValid(actionExecutedContext.Response))
            {
                actionExecutedContext.Response.TryGetContentValue(out responseObject);
                if (responseObject is IQueryable)
                {
                    var robj = responseObject as IQueryable<object>;
                    if (robj != null)
                    {
                        actionExecutedContext.Request.Headers.Add("Use-Response-Wrapper", "false");
                        if (_queryCount != null)
                        {
                            actionExecutedContext.Response =
                                actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK,
                                    new Metadata<IQueryable<object>>(robj, _queryCount) { StatusCode = 200 });
                        }
                        else
                        {
                            var result = robj.ToList();
                            actionExecutedContext.Response =
                                actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK,
                                    new Metadata<IEnumerable<object>>(result, result.Count) { StatusCode = 200 });
                        }
                    }
                }
            }
        }



        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            var url = queryOptions.Request.RequestUri.AbsoluteUri;
            if (queryOptions.Filter != null)
            {
                //queryOptions.ApplyTo(queryable);

                // timezone injection hack for kendodatetime filter
                url = url.Replace("%2B00%3A00", "%2B02%3A00");
                var req = new HttpRequestMessage(HttpMethod.Get, url);

                queryOptions = new ODataQueryOptions(queryOptions.Context, req);
            }

            var inlinecount = HttpUtility.ParseQueryString(queryOptions.Request.RequestUri.Query).Get("$count");
            if (!string.IsNullOrEmpty(inlinecount) && inlinecount.ToLower() == "true")
            {
                var filterRequest = new HttpRequestMessage(HttpMethod.Get,
                    url.Replace("$top", "top").Replace("$skip", "skip"));
                var filterQueryOptions = new ODataQueryOptions(queryOptions.Context, filterRequest);
                var originalQuery = filterQueryOptions.ApplyTo(queryable);
                _queryCount = (originalQuery as IQueryable<object>)?.Count();
            }

            var result = queryOptions.ApplyTo(queryable);
            return result;
        }

        private bool ResponseIsValid(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK || !(response.Content is ObjectContent))
            {
                return false;
            }
            return true;
        }
    }
}