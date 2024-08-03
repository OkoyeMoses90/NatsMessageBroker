using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NatsServer
{
    public class NatsServer
    {
        private readonly TcpListener _server;
        private readonly SubscriptionManager _subscriptionManager;

        //Listen for incoming connections
        public NatsServer(IPAddress ipAddress, int port)
        {
            _server = new TcpListener(ipAddress,port);
            _subscriptionManager = new SubscriptionManager();
        }

        //Initialize the server and accept incoming client connections
        public async Task StartAsync()
        {
            _server.Start();
            Console.WriteLine("NATS Server Started");

            while (true)
            {
                var client = await _server.AcceptTcpClientAsync();
                Console.WriteLine("Client connected");
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        //Handle client connection
        //Read the client data from stream and parse messages
        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            
            //Send INFO message
            var clientEndpoint = client.Client.RemoteEndPoint.ToString();
            var infoMessage = $"INFO {{\"host\": \"{_server.LocalEndpoint}\", \"port\": 4222, \"client_id\": \"{clientEndpoint}\"}}\r\n";
            await stream.WriteAsync(Encoding.UTF8.GetBytes(infoMessage));

            while(true)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                //Client disconnected
                if (bytesRead == 0)
                {
                    break;
                } 

                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                var parsedMessage = ParseMessage(message);

                switch (parsedMessage.MessageType)
                {
                    case NatsMessageType.Connect:
                        await stream.WriteAsync(Encoding.UTF8.GetBytes("+ok\r\n"));
                        break;
                    
                    case NatsMessageType.Ping:
                        await stream.WriteAsync(Encoding.UTF8.GetBytes("PONG\r\n"));
                        break;

                    case NatsMessageType.Sub:
                        if (parsedMessage.Parts.Length >= 3)
                        {
                            var topic = parsedMessage.Parts[1];
                            _subscriptionManager.Subscribe(topic, client);
                            Console.WriteLine("Client has subscribed to {topic}");
                        }
                        break;
                    
                    case NatsMessageType.Pub:
                        if (parsedMessage.Parts.Length >= 3)
                        {
                            var topic = parsedMessage.Parts[1];
                            var payload = message.Substring(message.IndexOf(parsedMessage.Parts[2]) + parsedMessage.Parts[2].Length).Trim();
                            await _subscriptionManager.Publish(topic, payload);
                            Console.WriteLine($"Published message to {topic} : {payload}");
                        }
                        break;


                    default:
                        Console.WriteLine("Default case");
                        break;
                }

            }
            client.Close();
            Console.WriteLine("Client Disconnected");
            
        }

        //Parse messages to identify their type
        public NatsMessage ParseMessage(string message)
        {
            var parts = message.Trim().Split(' ');
            var messageType = parts[0].ToUpper() switch
            {
                "CONNECT" => NatsMessageType.Connect,
                "PUB" => NatsMessageType.Pub,
                "SUB" => NatsMessageType.Sub,
                "UNSUB" => NatsMessageType.Unsub,
                "MSG" => NatsMessageType.Msg,
                "PING" => NatsMessageType.Ping,
                "PONG" => NatsMessageType.Pong,
                _ => NatsMessageType.Unknown
            };
            return new NatsMessage (messageType, parts);
        }

    }
}