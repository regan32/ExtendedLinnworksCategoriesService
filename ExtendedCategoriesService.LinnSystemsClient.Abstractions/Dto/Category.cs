namespace ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto
{
    using System;

    public class Category
    {
        public Guid CategoryId { get; set; }
        
        public string CategoryName { get; set; }
    }
}