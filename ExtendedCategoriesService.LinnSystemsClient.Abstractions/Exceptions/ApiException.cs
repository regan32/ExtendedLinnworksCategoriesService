namespace ExtendedCategoriesService.LinnSystemsClient.Abstractions.Exceptions
{
    using System;
    using System.Collections.Generic;

    public class ApiException : Exception
    {
        public string ResponseText { get; set; }
        public int StatusCode { get; set; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; set; }

        public ApiException(string message, int responseStatusCode, string responseText, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception ex)
        : base($"{message}: {responseText}", ex)
        {
            ResponseText = responseText;
            StatusCode = responseStatusCode;
            Headers = headers;
        }

        public ApiException(string message, int responseStatusCode, string responseText)
            : base($"{message}: {responseText}")
        {
           ResponseText = responseText;
           StatusCode = responseStatusCode;
        }
    }
}