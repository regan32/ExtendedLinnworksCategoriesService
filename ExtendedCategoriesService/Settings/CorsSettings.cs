namespace ExtendedCategoriesService.Settings
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Cors.Infrastructure;

    public sealed class CorsSettings
    {
        /// <summary>
        /// Gets the value indicating whether CORS has any registered policy.
        /// </summary>
        public bool Enabled => Policies.Count > 0;

        /// <summary>
        /// Gets or sets the value indicating whether CORS must be registered in the HTTP pipeline processing.
        /// </summary>
        public bool UseMiddleware { get; set; } = true;

        /// <summary>
        /// Gets the configured CORS policies
        /// </summary>
        public IDictionary<string, CorsPolicy> Policies { get; } = new Dictionary<string, CorsPolicy>();
    }
}