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


namespace WDev.AspNetCore.Middleware
{
    public class HttpLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLoggingMiddleware> _logger;

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            string reqInfo=string.Empty;    //Request Information ,Identifity Request and Depency Response.
            string reqStr=string.Empty;     //Request Head and Body AS String
            string resStr=string.Empty;     //Resposne Head and Body AS String
            var watch = Stopwatch.StartNew(); //Timer ,Recode Response Time Of Http Request Until Response.
            try
            {
                reqInfo = $"{context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path} {context.Request.QueryString}";               
                reqStr  = await FormatRequest(context.Request);
                _logger.LogDebug($"Request {reqInfo}{Environment.NewLine}{reqStr}");
                 var originalBodyStream = context.Response.Body;
                //Thread.Sleep(3000);            
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    resStr = await FormatResponse(context.Response);                    
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                _logger.LogDebug($"Response({context.Response.StatusCode}) {reqInfo} (Duration = {watch.Elapsed.Duration()}){Environment.NewLine}{resStr}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex,$"{reqInfo}{Environment.NewLine}{reqStr}{Environment.NewLine}{resStr}");
                throw ex;
            }finally{
                watch.Stop();          
            }
        }
        private string FormatHttpHeads(IHeaderDictionary head)
        {
            //Get Http Head Infos
            string headStr ="";
            head.AsParallel().ForAll( h =>
            {
                headStr += $"{h.Key}:{h.Value}{Environment.NewLine}";                
            });
            return headStr;
        }
        private async Task<string> FormatRequest(HttpRequest request)
        {
            var injectedRequestStream = new MemoryStream();            
            using (var bodyReader = new StreamReader(request.Body))
            {
                var bodyAsText = await bodyReader.ReadToEndAsync();               
                var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                request.Body = injectedRequestStream;
                return FormatHttpHeads(request.Headers) + $"{bodyAsText}";
            }
        }
        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);            
            var request = response.HttpContext.Request;            
            return FormatHttpHeads(response.Headers) + $"{bodyAsText}";
        }
    }
}
