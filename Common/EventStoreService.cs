using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Common
{
    public class EventStoreService : IDisposable
    {
        private readonly string _ipAddress;
        private readonly int _defaultPort;
        private IEventStoreConnection _connection;

        public EventStoreService(string ipAddress, int defaultPort)
        {
            _ipAddress = ipAddress;
            _defaultPort = defaultPort;
        }

        private void CreateConnectionAndConnect()
        {
            var ipAddressEndpoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _defaultPort);

            var settings = ConnectionSettings.Create();

            Console.WriteLine("Creating connection");
            _connection = EventStoreConnection.Create(settings, ipAddressEndpoint);
            _connection.ConnectAsync().Wait();
        }

        public EventStoreStreamCatchUpSubscription SubscribeToStream(string streamName
            , Action<EventStoreCatchUpSubscription, ResolvedEvent> callback)
        {
            if (_connection == null)
            {
                CreateConnectionAndConnect();
            }
            Console.WriteLine($"Subscribing to {streamName}");

            return _connection.SubscribeToStreamFrom(streamName, StreamPosition.Start, true, callback);
        }

        public Task<WriteResult> WriteToStreamAsync(string streamName, string eventType, object data, object metadata)
        {
            if (_connection == null)
            {
                CreateConnectionAndConnect();
            }

            var dataSerialized = Utility.StringToBytes(JsonConvert.SerializeObject(data));
            var metadataSerialized = Utility.StringToBytes(JsonConvert.SerializeObject(metadata));

            var eventData = new EventData(Guid.NewGuid(), eventType, true, dataSerialized, metadataSerialized);

            return _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData);
        }

        public T GetStreamMetaData<T>(string streamName)
        {
            if (_connection == null)
            {
                CreateConnectionAndConnect();
            }

            var metaData = _connection.GetStreamMetadataAsync(streamName).Result;
            return JsonConvert.DeserializeObject<T>(metaData.StreamMetadata.AsJsonString());
        }

        public void SetMetaData<T>(string streamName, T metadata)
        {
            if (_connection == null)
            {
                CreateConnectionAndConnect();
            }

            var seriliazedMetadata = Utility.StringToBytes(JsonConvert.SerializeObject(metadata));
            _connection.SetStreamMetadataAsync(streamName, ExpectedVersion.Any, seriliazedMetadata).Wait();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
