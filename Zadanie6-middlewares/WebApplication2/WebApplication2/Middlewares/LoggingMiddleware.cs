using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Services;

namespace WebApplication2.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IDbService service)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string sciezka = httpContext.Request.Path;
                string metoda = httpContext.Request.Method.ToString();
                string querystring = httpContext.Request?.QueryString.ToString();
                string bodyStr = "";

                using (var reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }
                string resultText = resultText.Concat(
                    sciezka, Environment.NewLine, 
                    metoda, Environment.NewLine,
                    querystring, Environment.NewLine,
                    bodyStr, Environment.NewLine
                );
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "requestsLog.txt"), true))
                {
                    outputFile.WriteLine(resultText);
                }
            }
            if (_next != null) await _next(httpContext);
        }
    }
}