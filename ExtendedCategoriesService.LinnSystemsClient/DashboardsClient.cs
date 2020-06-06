namespace ExtendedCategoriesService.LinnSystemsClient
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto;

    internal class DashboardsClient : BaseLinnWorksClient, IDashboardsClient
    {
        public DashboardsClient(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public Task<CustomScriptResult> ExecuteCustomScriptCustomerAsync(string script,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<CustomScriptResult>(
                "api/Dashboards/ExecuteCustomScriptQuery",
                HttpMethod.Post,
                body: "script=" + System.Net.WebUtility.UrlEncode(script),
                headers: headers,
                cancellationToken: cancellationToken);
        }
    }
}