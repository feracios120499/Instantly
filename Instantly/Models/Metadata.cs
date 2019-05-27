using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace Instantly.Models
{
    [DataContract]
    public class Metadata<T> where T : class
    {

        private void SetDefaulData()
        {
            Timestamp = DateTime.Now;
            Version = string.Format("v{0}", "1");
        }
        public Metadata()
        {
            SetDefaulData();
        }

        public Metadata(T data, long? total)
        {
            Total = total;
            Result = data;
            SetDefaulData();
        }
        [JsonConstructor]
        public Metadata(HttpResponseMessage httpResponse, bool isIQueryable)
        {
            SetDefaulData();
            if (httpResponse == null) return;

            StatusCode = (int)httpResponse.StatusCode;
            if (httpResponse.Content != null && httpResponse.IsSuccessStatusCode)
            {
                if (isIQueryable)
                {
                    IEnumerable<T> enumResponseObject;
                    httpResponse.TryGetContentValue(out enumResponseObject);
                    Results = enumResponseObject.ToArray();
                    ReturnedResults = enumResponseObject.Count();
                }
                else
                {
                    T responseObject;
                    var tryConvert = httpResponse.TryGetContentValue(out responseObject);
                    var enumerable = responseObject as IEnumerable<object>;
                    if (tryConvert && enumerable != null)
                    {
                        Total = enumerable.Count();
                    }
                    Result = responseObject;
                }
            }
            else
            {
                object content;
                if (httpResponse.TryGetContentValue(out content) && !httpResponse.IsSuccessStatusCode)
                {
                    var error = content as HttpError;
                    if (error != null)
                    {
                        if (error.InnerException != null)
                        {
                            error = error.InnerException;
                        }
                        ErrorMessage = error.Message;
                        ExceptionMessage = error.ExceptionMessage;

                        if (error.InnerException != null)
                        {
                            InnerException = new InnerException
                            {
                                Message = error.InnerException.Message,
                                ExceptionMessage = error.InnerException.ExceptionMessage,
                                MessageDetail = error.InnerException.MessageDetail,
                                ExceptionType = error.InnerException.ExceptionType,
                                //#if DEBUG    
                                StackTrace = error.InnerException.StackTrace
                                //#endif                                                            
                            };
                        }
                        //#if DEBUG
                        ErrorStackTrace = string.Format("{0}", Json.Encode(error));
                        //#endif
                    }
                }
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public string Version { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime Timestamp { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TotalResults { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int ReturnedResults { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public long? Total { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int StatusCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public T[] Results { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public T Result { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ExceptionMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public InnerException InnerException { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ErrorStackTrace { get; set; }
    }
    [DataContract]
    public class InnerException
    {
        [DataMember(EmitDefaultValue = false)]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ExceptionMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MessageDetail { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ExceptionType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string StackTrace { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string HelpLink { get; set; }
    }
}