using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tut6.MIddlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        FileStream file = new FileStream("C:\\Users\\Ego\\Desktop\\tut6\\tut6\\tut6\\Info\\requestInfo.txt", FileMode.Open);

        public LoggingMiddleware(RequestDelegate next) {
            _next = next; 
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path; // /api/students
                string method = httpContext.Request.Method; // GET, POST
                string queryString = httpContext.Request.QueryString.ToString();
                string bodyStr = "";

                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                using (StreamWriter writer = new StreamWriter(file))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    writer.Write(path);
                    writer.Write(method);
                    writer.Write(queryString);
                    writer.Write(bodyStr);
                    writer.Flush();
                    writer.Close();
                }
                

            }

            await _next(httpContext); 
        }
    }
}
