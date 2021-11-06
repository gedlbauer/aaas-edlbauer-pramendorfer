using AaaS.Common;
using AaaS.Dal.Ado;
using System;

namespace AaaS.Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationUtil.GetConfiguration();
            var factory = DefaultConnectionFactory.FromConfiguration(config, "PersonDbConnection");
            var detector = new MSSQLDetectorDao(factory);
            Console.WriteLine(detector);
        }
    }
}
