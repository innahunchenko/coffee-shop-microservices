using Azure.Messaging.ServiceBus;
using Messaging.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ProcessCheckoutAzFunc
{
    public class ProcessCheckout
    {
        private readonly ILogger<ProcessCheckout> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAsyncPolicy<HttpResponseMessage> _policy;

        public ProcessCheckout(ILogger<ProcessCheckout> logger, HttpClient httpClient, IConfiguration configuration, IAsyncPolicy<HttpResponseMessage> policy)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _policy = policy;
        }

        [Function(nameof(ProcessCheckout))]
        public async Task Run(
            [ServiceBusTrigger("checkout-msgs", Connection = "ServiceBusConnectionString")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            await ProcessMessageAsync(message, messageActions, isDeadLetter: false);
        }

        //[Function("ProcessCheckoutDeadLetter")]
        //public async Task RunDeadLetter(
        //    [ServiceBusTrigger("checkout-msgs/$DeadLetterQueue", Connection = "ServiceBusConnectionString")]
        //    ServiceBusReceivedMessage deadLetterMessage,
        //    ServiceBusMessageActions messageActions)
        //{
        //    await ProcessMessageAsync(deadLetterMessage, messageActions, isDeadLetter: true);
        //}

        private async Task ProcessMessageAsync(ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, bool isDeadLetter)
        {
            try
            {
                _logger.LogInformation("[{queue}] Processing message. ID: {id}", isDeadLetter ? "DLQ" : "MainQueue", message.MessageId);

                var jsonString = message.Body.ToString();
                _logger.LogInformation("[{queue}] Extracted JSON: {jsonString}", isDeadLetter ? "DLQ" : "MainQueue", jsonString);

                var checkoutData = JsonSerializer.Deserialize<CartCheckoutEvent>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (checkoutData == null)
                {
                    _logger.LogError("[{queue}] Deserialized message is null", isDeadLetter ? "DLQ" : "MainQueue");
                    return;
                }

                var orderApiUrl = _configuration["orderApiUrl"];
                var requestBody = new StringContent(JsonSerializer.Serialize(checkoutData), Encoding.UTF8, "application/json");
                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //var response = await _httpClient.PostAsync(orderApiUrl, requestBody);

                var response = await _policy.ExecuteAsync(() => _httpClient.PostAsync(orderApiUrl, requestBody));

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("[{queue}] Successfully processed message", isDeadLetter ? "DLQ" : "MainQueue");
                    await messageActions.CompleteMessageAsync(message);
                }
                else
                {
                    _logger.LogError("[{queue}] Failed to send order data. Status: {statusCode}, Response: {responseText}",
                        isDeadLetter ? "DLQ" : "MainQueue", response.StatusCode, await response.Content.ReadAsStringAsync());
                    await messageActions.AbandonMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{queue}] Error processing message {id}", isDeadLetter ? "DLQ" : "MainQueue", message.MessageId);
                await messageActions.AbandonMessageAsync(message);
            }
        }
    }
}
