using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.SeederClient
{
    internal class SampleClient
    {
        private readonly IClientDao clientDao;
        private readonly ILogDao logDao;
        private readonly IMetricDao metricDao;
        private readonly ITimeMeasurementDao timeMeasurementDao;

        public SampleClient(IClientDao clientDao, ILogDao logDao, IMetricDao metricDao, ITimeMeasurementDao timeMeasurementDao)
        {
            this.clientDao = clientDao;
            this.logDao = logDao;
            this.metricDao = metricDao;
            this.timeMeasurementDao = timeMeasurementDao;
        }

        public async Task PrintStats()
        {
            Console.WriteLine("--------------Sample AaaS Client--------------");

            Console.WriteLine("Registered Clients: ");
            await foreach (var item in clientDao.FindAllAsync())
            {
                Console.WriteLine($"{item.Id}: Name={item.Name}, ApiKey={item.ApiKey}");
            }

            Console.WriteLine("Top 10 Metrics:");
            await foreach (var item in metricDao.FindAllAsync().Take(10))
            {
                Console.WriteLine($"{item.Id}: Name={item.Name}, Value={item.Value}");
            }

            Console.WriteLine("Top 10 Logs:");
            await foreach (var item in logDao.FindAllAsync().Take(10))
            {
                Console.WriteLine($"{item.Id}: Name={item.Name} {item.Message}");
            }

            Console.WriteLine("Top 10 TimeMeasurement:");
            await foreach (var item in timeMeasurementDao.FindAllAsync().Take(10))
            {
                Console.WriteLine($"{item.Id}: Name={item.Name}, StartTime={item.StartTime:G} EndTime={item.EndTime:G}");
            }
        }
    }
}
