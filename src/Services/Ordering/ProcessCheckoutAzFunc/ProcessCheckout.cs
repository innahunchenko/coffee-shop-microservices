using Azure.Messaging.ServiceBus;
using Messaging.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public ProcessCheckout(ILogger<ProcessCheckout> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        [Function(nameof(ProcessCheckout))]
        public async Task Run(
            [ServiceBusTrigger("checkout-msgs", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.LogInformation("Message ID: {id}", message.MessageId);
                _logger.LogInformation("Raw Message Body Type: {type}", message.Body.GetType());

                var jsonString = message.Body.ToString();
                _logger.LogInformation("Extracted JSON string: {jsonString}", jsonString);

                var checkoutData = JsonSerializer.Deserialize<CartCheckoutEvent>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (checkoutData == null)
                {
                    _logger.LogError("Deserialized message is null");
                    return;
                }

                var orderApiUrl = _configuration["orderApiUrl"];

                var requestBody = new StringContent(JsonSerializer.Serialize(checkoutData), Encoding.UTF8, "application/json");
                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.PostAsync(orderApiUrl, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Order data successfully sent");
                    await messageActions.CompleteMessageAsync(message); 
                }
                else
                {
                    _logger.LogError("Failed to send order data. Status code: {statusCode}, Response: {responseText}",
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                    await messageActions.AbandonMessageAsync(message); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {id}", message.MessageId);
                await messageActions.DeadLetterMessageAsync(message); 
            }
        }
    }

}
