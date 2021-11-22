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
                Console.WriteLine($"{item.Id}: {item.Name}");
            }

            Console.WriteLine("Metrics:");
            Console.WriteLine($"{(await metricDao.FindAllAsync().CountAsync())} metrics in the database.");


            Console.WriteLine("Logs:");
            Console.WriteLine($"{(await logDao.FindAllAsync().CountAsync())} logs in the database.");

            Console.WriteLine("TimeMeasurement:");
            Console.WriteLine($"{(await timeMeasurementDao.FindAllAsync().CountAsync())} time measurements in the database.");
        }
    }
}
