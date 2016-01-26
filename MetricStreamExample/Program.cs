using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Models;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace MetricStreamExample
{
    class Program
    {
        static Random _rand = new Random();
        //write events from here
        static void Main(string[] args)
        {
            const string textMessageStream = "text-message";

            var service = new EventStoreService(Config.IpAddress, Config.DefaultPort);

           

            var people = new List<string> {"jim", "mike", "susie"};

            //simulate already subscribed. This information would come from the actual message in the form of it not being the first
            //phase transition or something else that tells us that this is not the first message related to an individuals
            var alreadySubscribed = new List<string>();

            var count = 0;
            using (service)
            {
                while (true)
                {
                    count++;
                    var person = people[count%3];
                    var model = BuildTextMessage(person);
                    var metadata = new Metadata { ClassVersion = 1 };

                    var subStream = $"{textMessageStream}-{person}";

                    if (alreadySubscribed.Contains(person) == false)
                    {
                        service.SubscribeToStream(subStream, (s, @event) =>
                        {
                            var metaData = service.GetStreamMetaData<Counter>(subStream);
                            var textMessage = JsonConvert.DeserializeObject<TextMessageSent>(Utility.BytesToString(@event.Event.Data));
                            Console.WriteLine($"Substream text message received for stream: {subStream}. Data {textMessage.Sender} - {textMessage.Message}");
                            Console.WriteLine($"Count is {metaData.Count}");
                            metaData.Count++;
                            service.SetMetaData(subStream, metaData);
                        });
                        alreadySubscribed.Add(person);
                    }

                    service.WriteToStreamAsync(textMessageStream, "messageSend", model, metadata);
                    service.WriteToStreamAsync(subStream, "messageSend", model, metadata);


                    Thread.Sleep(1000);
                }
            }
        }

        static TextMessageSent BuildTextMessage(string person)
        {
            var messages = new List<string>
            {
                "Oh hey there",
                "hi hi whats up?",
                "oh hi!"
            };

            var idx = _rand.Next(0, messages.Count);
            var msg = messages[idx];

            return new TextMessageSent
            {
                Date = DateTime.Now,
                Message = msg,
                Sender = person
            };
        }
    }

    public class Counter
    {
        public int Count { get; set; }
    }
}
