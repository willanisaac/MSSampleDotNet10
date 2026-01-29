namespace BFF.Domain.Constants;

public static class GraphQLConstants
{
    public const string QueryOperationType = "query";
    public const string MutationOperationType = "mutation";
    
    public static class Headers
    {
        public const string ContentType = "application/json";
        public const string TraceParent = "traceparent";
        public const string TraceState = "tracestate";
        public const string CorrelationId = "X-Correlation-ID";
    }
}
