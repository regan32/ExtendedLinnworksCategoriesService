namespace ExtendedCategoriesService.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtendedCategoriesService.WebApi.Abstractions;
    using ExtendedCategoriesService.WebApi.Abstractions.Dto;
    using ExtendedCategoriesService.WebApi.Filters;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]", Name = nameof(InventoryController))]
    [ApiController]
    [ServiceFilter(typeof(LinnWorksExceptionFilterAttribute))]
    public class InventoryController : ControllerBase
    {
        private readonly ICategoriesProxyService categoriesProxyService;

        public InventoryController(
            ICategoriesProxyService categoriesProxyService
            )
        {
            this.categoriesProxyService = categoriesProxyService;
        }

        [HttpGet]
        [Route("categories", Name = "GetCategories")]
        [ProducesResponseType(typeof(IEnumerable<Category>),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategoriesAsync(
            [Range(1, int.MaxValue)] int pageNumber = 1, 
            [Range(1, 100)] int pageSize = 100, 
            CancellationToken cancellationToken = default)
        {
            var result = await categoriesProxyService.GetCategoriesWithProducts(pageSize, pageNumber, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("categories", Name = "CreateCategory")]
        [ProducesResponseType(typeof(Category),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateCategoriesAsync(
            [FromBody] [Required] NewCategory category, 
            CancellationToken cancellationToken = default)
        {
            var result = await categoriesProxyService.CreateCategoryAsync(category.CategoryName, cancellationToken);
            return Ok(result);
        }
        
        [HttpDelete]
        [Route("categories", Name = "DeleteCategoryById")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteCategoriesAsync(
            [Required] Guid categoryId, 
            CancellationToken cancellationToken = default)
        {
            await categoriesProxyService.DeleteCategoryByIdAsync(categoryId, cancellationToken);
            return NoContent();
        }
    }
}