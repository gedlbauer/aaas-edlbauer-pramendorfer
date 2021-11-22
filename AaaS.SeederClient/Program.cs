using System;
using System.IO;

namespace AaaS.SeederClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DestroyDatabase();
        }

        private static void DestroyDatabase()
        {
            string[] data = File.ReadAllLines(@"C:\temp\AaaS\seeding\bulk_insert_clients.sql");
            foreach (var line in data)
            {
                Console.WriteLine(line);
            }
        }
    }
}
