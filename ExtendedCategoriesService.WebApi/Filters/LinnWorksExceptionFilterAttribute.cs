namespace ExtendedCategoriesService.WebApi.Filters
{
    using System;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Newtonsoft.Json.Linq;
    using Serilog;
    using StatusCodeResult = Microsoft.AspNetCore.Mvc.StatusCodeResult;

    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class LinnWorksExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger logger;

        public LinnWorksExceptionFilterAttribute(ILogger logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var apiException = context.Exception as ApiException;
            if (apiException == null)
            {
                return;
            }


            int statusCode = apiException.StatusCode;

            if (string.IsNullOrEmpty(apiException.ResponseText))
            {
                context.Result = new StatusCodeResult(statusCode);
            }
            else
            {
                var contentTypes = new MediaTypeCollection();

                foreach (var type in apiException.Headers["Content-Type"])
                {
                    contentTypes.Add(type);
                }

                
                context.Result = new JsonResult(JObject.Parse(apiException.ResponseText))
                {
                    StatusCode = statusCode,
                };
            }

            context.ExceptionHandled = true;

            logger.Warning(apiException.Message);
        }
    }
}