namespace ExtendedCategoriesService.WebApi.Abstractions.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class NewCategory
    {
        [MinLength(1)] 
        [MaxLength(128)]
        public string CategoryName { get; set; }
    }
}