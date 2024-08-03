using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NatsServer
{
    public class SubscriptionManager
    {
        private readonly ConcurrentDictionary<string, List<TcpClient>> _subscriptions;

        //Dictionary where keys are topic names and values are lists of clients subscribed to a topic.
        public SubscriptionManager()
        {
            _subscriptions = new ConcurrentDictionary<string,List<TcpClient>>();
        }

        //Subscribe a client to a topic 
        public void Subscribe(string topic, TcpClient client)
        {
            _subscriptions.AddOrUpdate(topic,
                _ => new List<TcpClient> { client },
                (_, clients) =>
                {
                    clients.Add(client);
                    return clients;
                });
        }

        //Remove a client from the list of subscribers
        public void Unsubscribe(string topic, TcpClient client)
        {
            if(_subscriptions.TryGetValue(topic, out var clients))
            {
                clients.Remove(client);
                if(clients.Count == 0)
                {
                    _subscriptions.TryRemove(topic, out _);
                }
            }
        }

        //Send message to all clients subscribed to a topic
        public async Task Publish(string topic, string message)
        {
            if(_subscriptions.TryGetValue(topic, out var clients))
            {
                var payload = Encoding.UTF8.GetBytes(message + "\r\n");
                foreach(var client in clients)
                {
                    if (client.Connected)
                    {
                        await client.GetStream().WriteAsync(payload, 0, payload.Length);
                    }
                }
            }
        }
    }
}