namespace ExtendedCategoriesService
{
    using System;
    using System.Reflection;
    using AutoMapper;
    using ExtendedCategoriesService.Extensions;
    using ExtendedCategoriesService.LinnSystemsClient;
    using ExtendedCategoriesService.WebApi;
    using ExtendedCategoriesService.WebApi.Filters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using ILogger = Serilog.ILogger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton(Log.Logger)
                .AddControllers()
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            var settings = Configuration.GetSwaggerSettings();
            if (settings?.Enabled == true)
            {
                serviceCollection.AddSwaggerGen((c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                    c.AddSecurityDefinition(SecuritySchemeType.ApiKey.ToString(), new OpenApiSecurityScheme {
                        In = ParameterLocation.Header, 
                        Description = "Please insert session id into field",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey 
                    });
                    
                    c.OperationFilter<AuthenticationRequirementsOperationFilter>();
                }));
            }
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddAutoMapper(e => e.AddMaps(AppDomain.CurrentDomain.GetAssemblies()), Assembly.GetEntryAssembly());
            serviceCollection.AddWebAPiExtensions();
            serviceCollection.AddHttpClients(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            logger.Information("Configuring application pipeline");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultConfiguration(Configuration);
        }
    }
}