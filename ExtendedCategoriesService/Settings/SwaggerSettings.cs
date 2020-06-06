namespace ExtendedCategoriesService.Settings
{
    public sealed class SwaggerSettings
    {
        /// <summary>
        /// Gets or sets whether swagger documentation should be enabled.
        /// Default is true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets route prefix
        /// Default is swagger.
        /// </summary>
        public string RoutePrefix { get; set; } = "swagger";
    }
}