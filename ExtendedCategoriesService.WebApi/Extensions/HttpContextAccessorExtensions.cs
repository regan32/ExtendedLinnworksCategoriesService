namespace ExtendedCategoriesService.WebApi.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    internal static class HttpContextAccessorExtensions
    {
        public static string GetAuthorization(this IHttpContextAccessor accessor)
        {
            if(accessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return authorization;
            }

            return null;
        }
    }
}