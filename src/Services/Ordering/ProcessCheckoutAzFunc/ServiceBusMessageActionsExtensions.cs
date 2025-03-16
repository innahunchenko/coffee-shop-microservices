namespace ProcessCheckoutAzFunc
{
    using Azure.Messaging.ServiceBus;
    using Microsoft.Azure.Functions.Worker;
    using System.Collections.Generic;

    public static class ServiceBusMessageActionsExtensions
    {
        public static async Task DeadLetterWithDetailsAsync(
            this ServiceBusMessageActions messageActions,
            ServiceBusReceivedMessage message,
            string reason,
            string description)
        {
            var deadLetterDetails = new Dictionary<string, object?>
        {
            { "DeadLetterReason", reason },
            { "DeadLetterErrorDescription", description }
        };

            await messageActions.DeadLetterMessageAsync(message, deadLetterDetails);
        }
    }
}
