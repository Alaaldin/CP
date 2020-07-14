using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace _01_SendEvents
{
    class Program
    {
        private const string connectionString = "Endpoint=sb://alaaldincp.servicebus.windows.net/;SharedAccessKeyName=my-sender;SharedAccessKey=WMd6erkOsexN28wP+bU9JLQMbuntmDKykkLkH78A7Wc=";
        private const string eventHubName = "new-distributer";
        static async Task Main()
        {
            // Create a producer client that you can use to send events to an event hub
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("{\"key\":\"1st event\"}")));
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("{\"key\":\"2nd event\"}")));
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("{\"key\":\"3rd event\"}")));

                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of 3 events has been published.");
            }
        }
    }
}