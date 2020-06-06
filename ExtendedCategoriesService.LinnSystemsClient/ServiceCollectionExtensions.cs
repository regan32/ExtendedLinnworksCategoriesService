namespace ExtendedCategoriesService.LinnSystemsClient
{
    using System;
    using System.Net.Http;
    using EnsureThat;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using Polly.Extensions.Http;
    using Polly.Retry;

    public static class ServiceCollectionExtensions
    {
        private static readonly AsyncRetryPolicy<HttpResponseMessage> DefaultPolicy =
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8),
                });

        public static IServiceCollection AddHttpClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(LinnSystemsClientSettings)).Get<LinnSystemsClientSettings>();
            
            EnsureArg.IsNotNullOrEmpty(settings?.Url, "LinnSystems url is not specified");

            var configureClient = new Action<IServiceProvider, HttpClient>((provider, client) => { client.BaseAddress = new Uri(settings.Url); });
                
            serviceCollection
                .AddHttpClientWithRetryPolicy<IDashboardsClient, DashboardsClient>(configureClient)
                .AddHttpClientWithRetryPolicy<ICategoriesClient, CategoriesClient>(configureClient);
            
            return serviceCollection;
        }

        private static IServiceCollection AddHttpClientWithRetryPolicy<TClient, TImplementation>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, HttpClient> configureClient = null,
            IAsyncPolicy<HttpResponseMessage> policy = null)
            where TClient : class
            where TImplementation : class, TClient
        {
            var retryPolicy = policy ?? DefaultPolicy;
            
            serviceCollection
                .AddHttpClient<TClient, TImplementation>(configureClient)
                .AddPolicyHandler(retryPolicy);
            
            return serviceCollection;
        }
    }
}