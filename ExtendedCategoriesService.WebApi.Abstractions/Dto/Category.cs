namespace ExtendedCategoriesService.WebApi.Abstractions.Dto
{
    using System;

    public class Category
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
        
        public long ProductsCount { get; set; }
    }
}