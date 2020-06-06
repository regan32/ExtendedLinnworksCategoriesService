namespace ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto
{
    using System.Collections.Generic;

    public class CustomScriptResult
    {
        public bool IsError;

        public string ErrorMessage;

        public long TotalResults;

        public IReadOnlyCollection<CustomScriptColumn> Columns;

        public IReadOnlyCollection<IDictionary<string, object>> Results;
    }
}