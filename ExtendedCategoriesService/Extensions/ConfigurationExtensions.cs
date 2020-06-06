namespace ExtendedCategoriesService.Extensions
{
    using ExtendedCategoriesService.Settings;
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        public const string CorsSettingsKey = "Cors";

        public static SwaggerSettings GetSwaggerSettings(this IConfiguration cfg)
        {
            return cfg.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        }

        public static CorsSettings GetCorsSettings(this IConfiguration cfg)
        {
            return cfg.GetSection(CorsSettingsKey).Get<CorsSettings>();
        }
    }
}