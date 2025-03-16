using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;

namespace ProcessCheckoutAzFunc
{
    public static class HttpClientConfig
    {
        public static void AddHttpClients(this IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"[Retry {retryAttempt}] Waiting {timespan.TotalSeconds}s due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    });

            //var circuitBreakerPolicy = HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .CircuitBreakerAsync(
            //        handledEventsAllowedBeforeBreaking: 3,
            //        durationOfBreak: TimeSpan.FromMinutes(1),
            //        onBreak: (outcome, breakDelay) =>
            //        {
            //            Console.WriteLine($"[Circuit Break] for {breakDelay.TotalSeconds}s due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            //        },
            //        onReset: () => Console.WriteLine("[Circuit Reset]"),
            //        onHalfOpen: () => Console.WriteLine("[Circuit Half-Open]")
            //    );

           // var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            services.AddHttpClient("OrderApiClient")
                .AddPolicyHandler(retryPolicy);

            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(retryPolicy);
        }
    }
}
