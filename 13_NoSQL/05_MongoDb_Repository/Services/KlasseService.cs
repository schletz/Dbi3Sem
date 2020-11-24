using MongoDbDemo.Domain;
using MongoDbDemo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDbDemo.Services
{
    class KlasseService
    {
        private readonly Repository repo = new Repository("mongodb://127.0.0.1:27017", "Stundenplan");
        public Klasse GetClassDetails(string klasse)
        {
            // Darf der User die Klasse überhaupt lesen?
            // Inkludiere die referenzierten Daten
            // Loggen?
            // Exception
            var foundClass = repo.FindById<Klasse>("1BHIF");
            foundClass.Kv = repo.FindById<Lehrer>(foundClass.KvId);
            return foundClass;
        }

        public void UpdateKv(string klasse, string lehrerId)
        {
            var found = repo.FindById<Klasse>(klasse);
            // BUSINESS LOGIK
            // Ein Lehrer darf nur KV einer Klasse sein.
            var l = repo.FindById<Lehrer>(lehrerId) ??
                throw new KlasseServiceException($"Der Lehrer {lehrerId} existiert nicht.");

            if (!repo.Find<Klasse>().Any(k => k.KvId == lehrerId))
            {
                found.KvId = lehrerId;
                repo.Update(found);
            }
            else
            {
                throw new KlasseServiceException($"Der Lehrer {lehrerId} ist bereits Klassenvorstand.");
            }
        }
    }
}