using Polly;
using Polly.Timeout;

namespace BFF.Infrastructure.Resilience;

public static class TimeoutPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            timeout: TimeSpan.FromSeconds(30),
            timeoutStrategy: TimeoutStrategy.Pessimistic,
            onTimeoutAsync: (context, timespan, task) =>
            {
                Console.WriteLine($"Request timed out after {timespan.TotalSeconds}s");
                return Task.CompletedTask;
            });
    }
}
