using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Actions
{
    public class WebHookAction : BaseAction
    {
        public string RequestUrl { get; set; }

        public async override Task Execute()
        {
            using var client = new HttpClient();
            await client.GetAsync(RequestUrl);
        }
    }
}
