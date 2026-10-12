using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => (int)msg.StatusCode == 429) // Handle Too Many Requests
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log retry attempt here if needed
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1),
                onBreak: (result, timespan) =>
                {
                    // Log when the circuit is broken
                },
                onReset: () =>
                {
                    // Log when the circuit resets
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(10); // Timeout after 10 seconds
    }

    public static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy()
    {
        return Policy.WrapAsync(GetRetryPolicy(), GetCircuitBreakerPolicy(), GetTimeoutPolicy());
    }
}
