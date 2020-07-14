using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace github
{
    public static class distributerfunc
    {
        [FunctionName("distributerfunc")]
         
        public static async Task Run([EventHubTrigger("new-distributer", Connection ="EventHubConnectionAppSetting")] Azure.Messaging.EventHubs.EventData[] events, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("EventHubConnectionAppSetting");
            string eventHubName = "reciver1";
            var exceptions = new List<Exception>();
           
             // Create a producer client that you can use to send events to an event hub
            var producerClient = new EventHubProducerClient(connectionString, eventHubName);
        
            // Create a batch of events 
            using Azure.Messaging.EventHubs.Producer.EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
            eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes("First event")));
            eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes("Second event")));
            eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes("Third event")));

            // Use the producer client to send the batch of events to the event hub
            await producerClient.SendAsync(eventBatch);
            Console.WriteLine("A batch of 3 events has been published.");
        
           

            foreach (Azure.Messaging.EventHubs.EventData eventData in events)
            {
                try
                {
                    // string messageBody = Encoding.UTF8.GetString(eventData.Body.ToArray,
                    //                                              eventData.Body.Offset,
                    //                                              eventData.Body.Count);

                    // // Replace these two lines with your processing logic.
                    // log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
