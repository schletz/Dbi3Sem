using Bogus;
using Bogus.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbDemo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDbDemo.Infrastructure
{
    // Stellt CRUD Operationen für eine Collection bereit.
    class Repository
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase db;
        public Repository(string connection, string database)
        {
            client = new MongoClient(connection);
            db = client.GetDatabase(database);
        }

        public IMongoQueryable<T> Find<T>()
        {
            return db.GetCollection<T>(typeof(T).Name)
                .AsQueryable();
        }
        public T FindById<T>(object id)
        {
            return db.GetCollection<T>(typeof(T).Name).Find(
                Builders<T>.Filter.Eq("_id", id)
            ).FirstOrDefault();
        }
        public void Update<T>(T item)
        {
            var prop = typeof(T).GetProperty("Id");
            var id = prop.GetValue(item);

            db.GetCollection<T>(typeof(T).Name)
                .ReplaceOne(Builders<T>.Filter.Eq("_id", id), item);
        }

        public void Delete<T>(T item)
        {
            var prop = typeof(T).GetProperty("Id");
            var id = prop.GetValue(item);

            db.GetCollection<T>(typeof(T).Name)
                .DeleteOne(Builders<T>.Filter.Eq("_id", id));
        }
        public void Insert<T>(T l)
        {
            db.GetCollection<T>(nameof(T)).InsertOne(l);
        }

        public void Seed()
        {
            db.DropCollection(nameof(Lehrer));
            db.DropCollection(nameof(Klasse));

            Randomizer.Seed = new Random(2112);
            var lehrer = new Faker<Lehrer>().Rules((f, l) =>
            {
                l.Vorname = f.Name.FirstName();
                l.Zuname = f.Name.LastName();
                l.Id = l.Zuname.Substring(0, 3).ToUpper();
                l.Email = $"{l.Zuname}@spengergasse.at".OrDefault(f, 0.2f);
            })
                .Generate(100)
                .ToHashSet()
                .ToList();

            db.GetCollection<Lehrer>(nameof(Lehrer)).InsertMany(lehrer);

            var abteilungen = new string[] { "HIF", "AIF", "BIF" };

            var klassen = new Faker<Klasse>().Rules((f, k) =>
            {
                k.Id = f.Random.Int(1, 5) + f.Random.String2(1, "ABCD") + f.Random.ListItem(abteilungen);
                k.KvId = f.Random.ListItem(lehrer).Id;
            })
                .Generate(10)
                .ToHashSet()
                .ToList();

            db.GetCollection<Klasse>(nameof(Klasse)).InsertMany(klassen);
        }
    }
}
