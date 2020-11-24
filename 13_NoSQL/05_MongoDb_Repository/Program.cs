using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDemo.Domain;
using MongoDbDemo.Services;

namespace MongoDbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new KlasseService();
            var foundClass = service.GetClassDetails("1BHIF");
            Console.WriteLine($"Der KV ist {foundClass.Kv.Zuname}");

            try
            {
                service.UpdateKv("1BHIF", "CAR");
            }
            catch (KlasseServiceException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
