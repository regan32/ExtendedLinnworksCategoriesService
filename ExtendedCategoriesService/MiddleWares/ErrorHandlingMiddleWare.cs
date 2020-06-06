namespace ExtendedCategoriesService.MiddleWares
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Serilog;
    using Serilog.Context;
    using ILogger = Serilog.ILogger;

    internal sealed class ErrorHandlingMiddleware
    {
        private readonly ILogger logger;
        private readonly RequestDelegate next;

        private readonly Dictionary<Type, HttpStatusCode> exceptionToCodeMap = new Dictionary<Type, HttpStatusCode>
        {
            { typeof(NotSupportedException),  HttpStatusCode.BadRequest },
            { typeof(ArgumentException),  HttpStatusCode.BadRequest },
            { typeof(UnauthorizedAccessException),  HttpStatusCode.Unauthorized },
            { typeof(OperationCanceledException), HttpStatusCode.Gone },
        };

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            logger = Log.Logger;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
            {
                Log.Information($"Received request to {context.Request.Path}");
            
                try
                {
                    await next.Invoke(context);
                    Log.Information(($"API response {context.Request.Path}, {context.Response.StatusCode}"));
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            }
            
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exceptionToCodeMap.TryGetValue(exception.GetType(), out var code) == false)
            {
                code = HttpStatusCode.InternalServerError;
            }
            
            logger.Error(exception, $"Request {context.Request.Path} execution failed");
            
#if DEBUG
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
#else
            var result = JsonConvert.SerializeObject(new { error = "Unexpected Error" });
#endif
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}