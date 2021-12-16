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
            var demoClient = new AaaSDemo(new AaaSService(client, new AaaSServiceOptions
            {
                ApiKey = apiKey
            }), eventsPerMinute);

            await demoClient.Start();
        }
    }
}
