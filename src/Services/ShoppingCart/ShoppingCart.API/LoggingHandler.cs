namespace ShoppingCart.API
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Request: {request}");
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine($"Response: {response}");
            return response;
        }
    }
}
