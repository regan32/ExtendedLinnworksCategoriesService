namespace ExtendedCategoriesService.Extensions
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    public static class WebHostBuilderExtensions
    {
        private const string KestrelSectionName = "Kestrel";
        
        public static IWebHostBuilder UseDefaultServices(
            this IWebHostBuilder builder)
        {
            return builder
                .UseKestrel((builderContext, options) =>
                {
                    options.AddServerHeader = false;

                    // Configure Kestrel from appsettings.json.
                    var kestrelSection = builderContext.Configuration.GetSection(KestrelSectionName);
                    options.Configure(kestrelSection);

                    // Configuring Limits from appsettings.json is not supported.
                    // See https://github.com/aspnet/KestrelHttpServer/issues/2216
                    var kestrelOptions = kestrelSection.Get<KestrelServerOptions>();
                    if (kestrelOptions != null)
                    {
                        options.AddServerHeader = kestrelOptions.AddServerHeader;
                        options.AllowSynchronousIO = kestrelOptions.AllowSynchronousIO;
                        foreach (var property in typeof(KestrelServerLimits).GetProperties())
                        {
                            if (property.CanWrite)
                            {
                                var value = property.GetValue(kestrelOptions.Limits);
                                property.SetValue(options.Limits, value);
                            }
                        }
                    }
                })
                .UseSerilog();
        }
    }
}