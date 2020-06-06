namespace ExtendedCategoriesService.LinnSystemsClient.Abstractions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto;

    public interface IDashboardsClient
    {
        Task<CustomScriptResult> ExecuteCustomScriptCustomerAsync(string script,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers,
            CancellationToken cancellationToken = default);
    }
}