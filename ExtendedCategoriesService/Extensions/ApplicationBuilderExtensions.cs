namespace ExtendedCategoriesService.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ExtendedCategoriesService.MiddleWares;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add usage of default middlewares to the <see cref="IApplicationBuilder"/>: Swagger, ErrorHandlingMiddleware, Cors, MVC.
        /// Note, that this method must be called all the others.
        /// </summary>
        /// <param name="app">The application pipeline builder</param>
        /// <param name="configuration">The host configuration</param>
        /// <param name="swaggerSetup">The setup callback for swagger</param>
        /// <param name="swaggerUISetup">The setup callback for swagger UI.</param>
        /// <returns>The same builder</returns>
        public static IApplicationBuilder UseDefaultConfiguration(
            this IApplicationBuilder app,
            IConfiguration configuration,
            Action<SwaggerOptions> swaggerSetup = null,
            Action<SwaggerUIOptions> swaggerUISetup = null)
        {
            app.UseRouting();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            var corsSettings = configuration.GetCorsSettings();
            if (corsSettings?.Enabled == true && corsSettings?.UseMiddleware == true)
            {
                app.UseCors(corsSettings.Policies.First().Key);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSwaggerAndUI(configuration, swaggerSetup, swaggerUISetup);

            return app;
        }

        public static IApplicationBuilder UseSwaggerAndUI(
            this IApplicationBuilder app,
            IConfiguration configuration,
            Action<SwaggerOptions> swaggerSetup,
            Action<SwaggerUIOptions> swaggerUISetup)
        {
            var settings = configuration.GetSwaggerSettings();
            if (settings?.Enabled != true)
            {
                return app;
            }

            app.UseSwagger(c =>
            {
                var prefixWithEndSlash =
                    string.IsNullOrEmpty(settings.RoutePrefix) ? string.Empty : settings.RoutePrefix + "/";

                if (swaggerSetup == null)
                {
                    c.RouteTemplate = prefixWithEndSlash + "{documentName}/swagger.json";
                }
                else
                {
                    swaggerSetup(c);
                    c.RouteTemplate = prefixWithEndSlash + c.RouteTemplate;
                }
            });

            app.UseSwaggerUI(c =>
            {
                var prefixWithStartSlash =
                    string.IsNullOrEmpty(settings.RoutePrefix) ? string.Empty : "/" + settings.RoutePrefix;

                if (swaggerUISetup == null)
                {
                    c.SwaggerEndpoint(prefixWithStartSlash + "/v1/swagger.json", "My API v1");
                }
                else
                {
                    swaggerUISetup(c);
                    if (c.ConfigObject.Urls != null && !string.IsNullOrEmpty(prefixWithStartSlash))
                    {
                        var newEndpointList = new List<UrlDescriptor>();
                        foreach (var endpoint in c.ConfigObject.Urls)
                        {
                            newEndpointList.Add(new UrlDescriptor
                            {
                                Url = prefixWithStartSlash + endpoint.Url,
                                Name = endpoint.Name,
                            });
                        }

                        c.ConfigObject.Urls = newEndpointList;
                    }
                }

                c.RoutePrefix = settings.RoutePrefix;
            });

            return app;
        }
    }
}