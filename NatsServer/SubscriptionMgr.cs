using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NatsServer
{
    public class SubscriptionManager
    {
        private readonly Dictionary<string, List<TcpClient>> _subscriptions = new();

        //Add a client to the list of subscribers
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

        //Send message to all clients subscribes to a topic
        public void Publish(string topic, string message)
        {
            if(_subscriptions.ContainsKey(topic))
            {
                var clients = _subscriptions[topic];
                foreach(var client in clients)
                {
                    var networkStream = client.GetStream();
                    var response = Encoding.UTF8.GetBytes(message);
                    networkStream.WriteAsync(response, 0, response.Length);
                }
            }
        }
    }
}