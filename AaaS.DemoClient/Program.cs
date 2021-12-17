using AaaS.ClientSDK;
using AaaS.ClientSDK.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AaaS.DemoClient
{
    class Program
    {

        const int eventsPerMinute = 30;
        const string apiKey = "d1447938-c1bd-453d-a2a8-bb1b895fbe65";
        static async Task Main(string[] args)
        {
            using HttpClient client = new HttpClient();
            Guid guid = Guid.Parse("417a4011-1a53-4a76-a697-5e488ac068fc");
            AaaSService.Create(client, new AaaSServiceOptions { ApiKey = apiKey, CreatorId = guid });
            var demoClient = new AaaSDemo(AaaSService.Instance, eventsPerMinute);

            demoClient.Start();

            Console.WriteLine("Client running");
            Console.WriteLine("Press Escape to stop client");
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                await AaaSService.Instance.Stop();
            }
        }
    }
}
