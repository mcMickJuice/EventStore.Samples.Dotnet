using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Models;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace MetricStreamListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var METRIC_STREAM = Config.MetricStream;
            var AGG_STREAM = Config.SecondaryStream;

            var service = new EventStoreService(Config.IpAddress, Config.DefaultPort);

            var streamSub = service.SubscribeToStream(METRIC_STREAM, (s, @event) =>
            {
                var data = Utility.BytesToString(@event.Event.Data);
                Console.WriteLine($"Received: {@event.Event.EventStreamId}. Event Number: {@event.Event.EventNumber}");
                var message = JsonConvert.DeserializeObject<CompletedMessage>(data);

                Console.WriteLine($"Completed Message Created on: {message.Date.ToShortTimeString()}");
            });


            var subStreamSub = service.SubscribeToStream(AGG_STREAM, (s, @event) =>
            {
                var data = Utility.BytesToString(@event.Event.Data);
                Console.WriteLine(
                    $"Received Secondary Event: {@event.Event.EventStreamId}. Event Number: {@event.Event.EventNumber}");
                var message = JsonConvert.DeserializeObject<CompletedMessage>(data);

                Console.WriteLine($"Completed Message Created on: {message.Date.ToShortTimeString()}");
            });

            Console.WriteLine("Waiting for events. Hit enter to exit");
            Console.ReadKey();
        }

    }
}

