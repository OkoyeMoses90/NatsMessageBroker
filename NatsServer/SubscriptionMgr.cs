using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NatsServer
{
    public class SubscriptionManager
    {
        private readonly Dictionary<string, List<TcpClient>> _subscriptions;

        //Dictionary where keys are topic names and values are lists of clients subscribed to a topic.
        public SubscriptionManager()
        {
            _subscriptions = new Dictionary<string,List<TcpClient>>();
        }

        //Subscribe a client to a topic 
        public void Subscribe(string topic, TcpClient client)
        {
            if(!_subscriptions.ContainsKey(topic))
            {
                _subscriptions[topic] = new List<TcpClient>();
            }
            _subscriptions[topic].Add(client);
        }

        //Remove a client from the list of subscribers
        public void Unsubscribe(string topic, TcpClient client)
        {
            if(_subscriptions.ContainsKey(topic))
            {
                _subscriptions[topic].Remove(client);
                if(_subscriptions[topic].Count == 0)
                {
                    _subscriptions.Remove(topic);
                }
            }
        }

        //Send message to all clients subscribed to a topic
        public async Task Publish(string topic, string message)
        {
            if(_subscriptions.ContainsKey(topic))
            {
                var clients = _subscriptions[topic];
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