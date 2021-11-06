using AaaS.Dal.Ado;
using System;

namespace AaaS.Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new SimpleDetector().GetType().AssemblyQualifiedName);
            var detector = AdoDetectorDao.MapRowToDetector(GetReturnRow());
        }
        private static MockDataRecord GetReturnRow()
        {
            return new MockDataRecord();
        }
    }
}
