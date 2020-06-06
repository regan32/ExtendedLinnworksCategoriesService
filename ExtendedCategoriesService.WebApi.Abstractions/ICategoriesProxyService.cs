namespace ExtendedCategoriesService.WebApi.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtendedCategoriesService.WebApi.Abstractions.Dto;

    public interface ICategoriesProxyService
    {
        Task<IEnumerable<Category>> GetCategoriesWithProducts(int pageSize, int pageNumber, CancellationToken cancellationToken = default);
        Task<Category> CreateCategoryAsync(string name, CancellationToken cancellationToken = default);
        Task DeleteCategoryByIdAsync(Guid categoriId, CancellationToken cancellationToken = default);
    }
}