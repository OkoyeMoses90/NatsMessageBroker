using System;
using System.Net;
using System.Threading.Tasks;

namespace NatsServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new NatsServer(IPAddress.Any, 4222);
            await server.StartAsync();
        }
    }
    
}