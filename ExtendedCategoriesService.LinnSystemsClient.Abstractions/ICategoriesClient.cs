namespace ExtendedCategoriesService.LinnSystemsClient.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto;

    public interface ICategoriesClient
    {
        Task<Category> CreateCategoryAsync(string name, IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers, CancellationToken cancellationToken = default);
        Task DeleteCategoryByIdAsync(Guid categoriId, IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers, CancellationToken cancellationToken = default);
    }
}