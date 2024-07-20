using System;
using System.Net.Sockets;
using System.Text;

namespace NatsServer
{
    public static class MessageHandler
    {
        //Route a message to the appropriate handles based on message type
        public static void HandleMessage(NatsMessage message, TcpClient client, SubscriptionManager subscriptionManager)
        {
            switch(message.MessageType)
            {
                case NatsMessageType.Connect:
                    HandleConnect(message,client);
                    break;
                case NatsMessageType.Pub:
                    HandlePub(message, subscriptionManager);
                    break;
                case NatsMessageType.Sub:
                    HandleSub(message, client, subscriptionManager);
                    break;
                case NatsMessageType.Unsub:
                    HandleUnsub(message, client, subscriptionManager);
                    break;
                case NatsMessageType.Ping:
                    HandlePing(client);
                    break;
                case NatsMessageType.Pong:
                    HandlePong(client);
                    break;
                default:
                    Console.WriteLine("Unknows message type received");
                    break;
                
            }
        }

        private static void HandleConnect(NatsMessage message, TcpClient client)
        {
            Console.WriteLine("Connect message received");
        }

        private static void HandlePub(NatsMessage message, SubscriptionManager subscriptionManager)
        {
            Console.WriteLine("Publish message received");
            var topic = message.Parts[1];
            var payload = string.Join(' ', message.Parts[2..]);
            subscriptionManager.Publish(topic, payload);
        }

        private static void HandleSub(NatsMessage message, TcpClient client, SubscriptionManager subscriptionManager)
        {
            Console.WriteLine("Subscription message received");
            var topic = message.Parts[1];
            subscriptionManager.Subscribe(topic,client);
        }

        private static void HandleUnsub(NatsMessage message, TcpClient client, SubscriptionManager subscriptionManager)
        {
            Console.WriteLine("Unsubscribe message received");
            var topic = message.Parts[1];
            subscriptionManager.Unsubscribe(topic, client);
        }

        private static void HandlePing(TcpClient client)
        {
            Console.WriteLine("Ping message received");
            var pingResponse = Encoding.UTF8.GetBytes("PONG\r\n");
            client.GetStream().WriteAsync(pingResponse, 0, pingResponse.Length);
        }

        private static void HandlePong(TcpClient client)
        {
            Console.WriteLine("Pong message received");
            var pongResponse = Encoding.UTF8.GetBytes("PONG\r\n");
            client.GetStream().WriteAsync(pongResponse, 0, pongResponse.Length);
        }
    }
}