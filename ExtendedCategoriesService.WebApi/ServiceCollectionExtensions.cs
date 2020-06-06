namespace ExtendedCategoriesService.WebApi
{
    using ExtendedCategoriesService.WebApi.Abstractions;
    using ExtendedCategoriesService.WebApi.Filters;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAPiExtensions(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<LinnWorksExceptionFilterAttribute>()
                .AddTransient<ICategoriesProxyService, CategoriesProxyService>();
            
            return serviceCollection;
        }
        
    }
}