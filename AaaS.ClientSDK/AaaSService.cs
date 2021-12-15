using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.ClientSDK
{
    public class AaaSService
    {
        private const string baseUrl = "https://localhost:5001";
        private readonly string _apiKey;
        public AaaSService(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task InsertLog(LogInsertDto log)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
            await new Client(baseUrl, client).LogsAsync(log);
        }

        public async Task InsertTimeMeasurement(TimeMeasurementInsertDto timeMeasurement)
        {
            using HttpClient client = new();
            await new Client(baseUrl, client).TimeMeasurementsAsync(timeMeasurement);
        }
        public async Task InsertCounter(CounterInsertDto counter)
        {
            using HttpClient client = new();
            await new Client(baseUrl, client).CountersAsync(counter);
        }
        public async Task InsertMeasurement(MeasurementInsertDto measurement)
        {
            using HttpClient client = new();
            await new Client(baseUrl, client).MeasurementsAsync(measurement);
        }
    }
}
