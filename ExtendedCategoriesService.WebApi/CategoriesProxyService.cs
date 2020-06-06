namespace ExtendedCategoriesService.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using EnsureThat;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Exceptions;
    using ExtendedCategoriesService.WebApi.Abstractions;
    using ExtendedCategoriesService.WebApi.Abstractions.Dto;
    using ExtendedCategoriesService.WebApi.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    internal class CategoriesProxyService : ICategoriesProxyService
    {
        internal static string PagedCategoryWithProductsQueryFormat =    
            "SELECT pc.CategoryName, " +
            "pc.CategoryId, " +
            "(SELECT COUNT(*) FROM StockItem WHERE CategoryId = pc.CategoryId) as [ProductsCount]," +
            "TotalRows = COUNT_BIG(*) OVER() " +
            "from ProductCategories pc " +
            "Order By pc.CategoryName " +
            "OFFSET {0} * ({1} - 1) ROWS " +
            "FETCH NEXT {0} ROWS ONLY;";

        private readonly ICategoriesClient categoriesClient;
        private readonly IDashboardsClient dashboardsClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public CategoriesProxyService(ICategoriesClient categoriesClient, IDashboardsClient dashboardsClient, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.categoriesClient = categoriesClient;
            this.dashboardsClient = dashboardsClient;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProducts(int pageSize, int pageNumber, CancellationToken cancellationToken = default)
        {
            EnsureArg.IsInRange(pageSize, 1, 100, nameof(pageSize));
            EnsureArg.IsGte(pageSize, 0, nameof(pageSize));
            
            var result = await dashboardsClient.ExecuteCustomScriptCustomerAsync(
                string.Format(PagedCategoryWithProductsQueryFormat, pageSize, pageNumber),
                GetHeaders(),
                cancellationToken);
            
            if (result.IsError)
            {
                throw new ApiException("Execution failed with error: ", (int)HttpStatusCode.BadRequest, result.ErrorMessage);
            }

            return mapper.Map<IEnumerable<Category>>(result);
        }

        public async Task<Category> CreateCategoryAsync(string name, CancellationToken cancellationToken = default)
        {
            EnsureArg.IsNotNullOrEmpty(name, nameof(name));
            
            var category = await categoriesClient.CreateCategoryAsync(name, GetHeaders(), cancellationToken);
            return mapper.Map<Category>(category);
        }

        public Task DeleteCategoryByIdAsync(Guid categoriId, CancellationToken cancellationToken = default)
        {
            EnsureArg.IsNotDefault(categoriId, nameof(categoriId));
            
            return categoriesClient.DeleteCategoryByIdAsync(categoriId,GetHeaders(), cancellationToken);
        }

        private IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetHeaders()
        {
            var headers = new Dictionary<string, IReadOnlyCollection<string>>();
            var authorization = httpContextAccessor.GetAuthorization();

            if (!string.IsNullOrWhiteSpace(authorization))
            {
                headers.Add(HeaderNames.Authorization, new []{ authorization });
            }

            return headers;
        }
    }
}