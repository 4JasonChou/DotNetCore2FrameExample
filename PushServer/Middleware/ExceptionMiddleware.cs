using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using PushServer.Exceptions;

namespace WDev.AspNetCore.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = JsonConvert.SerializeObject(new { error = exception.Message });

            if (exception is CustomException) {
                var ex = (CustomException)exception;
                var res = new FailedResopnse(ex.getStatusCode,ex.getStatusMsg);
                code = (HttpStatusCode)res.ErrorCode;
                result = JsonConvert.SerializeObject(res);
            }
            else {
                var res = new FailedResopnse(550,exception.Message);
                code = (HttpStatusCode)res.ErrorCode;
                result = JsonConvert.SerializeObject(res);
            }


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }

    public class FailedResopnse {
        public int ErrorCode;
        public string ErrorMsg;
        public FailedResopnse(int code,string msg) {
            ErrorCode = code; 
            ErrorMsg = msg;
        }
    }
}
