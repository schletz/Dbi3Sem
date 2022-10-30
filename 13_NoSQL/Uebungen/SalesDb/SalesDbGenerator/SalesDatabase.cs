using Bogus;
using DnsClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesDbGenerator
{
    /// <summary>
    /// Unit of Work für die Exam Datenbank. Stellt alle Collections als Property bereit,
    /// damit auf diese zugegriffen werden kann. Änderungen bei den Namen können so sehr einfach
    /// gemacht werden.
    /// </summary>
    class SalesDatabase
    {
        public MongoClient Client { get; }
        public IMongoDatabase Db { get; }
        public IMongoCollection<Product> Products => Db.GetCollection<Product>("products");
        public IMongoCollection<Customer> Customers => Db.GetCollection<Customer>("customers");
        public IMongoCollection<Order> Orders => Db.GetCollection<Order>("orders");

        /// <summary>
        /// Konfiguriert die Datenbank für einen Connection string.
        /// </summary>
        public static SalesDatabase FromConnectionString(string connectionString, bool logging = false)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            if (logging)
            {
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        // Bei update Statements geben wir die Anweisung aus, wie wir sie in
                        // der Shell eingeben könnten.
                        if (e.Command.TryGetValue("updates", out var updateCmd))
                        {
                            var collection = e.Command.GetValue("update");
                            var isUpdateOne = updateCmd[0]["q"].AsBsonDocument.Contains("_id");
                            foreach (var cmd in updateCmd.AsBsonArray)
                            {
                                Console.WriteLine($"db.getCollection(\"{collection}\").{(isUpdateOne ? "updateOne" : "updateMany")}({updateCmd[0]["q"]}, {updateCmd[0]["u"]})");
                            }
                        }
                        // Bei aggregate Statements geben wir die Anweisung aus, wie wir sie in
                        // der Shell eingeben könnten.
                        if (e.Command.TryGetValue("aggregate", out var aggregateCmd))
                        {
                            var collection = aggregateCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").aggregate({e.Command["pipeline"]})");
                        }

                        // Bei Filter Statements geben wir die find Anweisung aus.
                        if (e.Command.TryGetValue("find", out var findCmd))
                        {
                            var collection = findCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").find({e.Command["filter"]})");
                        }
                    });
                };
            }
            var client = new MongoClient(settings);
            var db = client.GetDatabase("salesDb");
            // LowerCase property names.
            var conventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(ignoreIfNull: true)
            };
            ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventions, _ => true);
            return new SalesDatabase(client, db);
        }

        private SalesDatabase(MongoClient client, IMongoDatabase db)
        {
            Client = client;
            Db = db;
        }

        /// <summary>
        /// Löscht die Datenbank und befüllt sie mit Musterdaten.
        /// </summary>
        public void Seed()
        {
            Db.DropCollection("products");
            Db.DropCollection("customers");
            Db.DropCollection("orders");

            Randomizer.Seed = new Random(944);
            var faker = new Faker("de");
            var categories = new string[]
            {
                "Sportswear",
                "Books",
                "Electronics"
            };
            var states = new Dictionary<string, (int zip, string name)[]>()
            {
                {"W", new (int, string)[] {(1010, "Wien"), (1020, "Wien"), (1030, "Wien"), (1040, "Wien") } },
                {"N", new (int, string)[] {(2500, "Baden"), (3100, "Sankt Pölten"), (3300, "Amstetten") } },
                {"B", new (int, string)[] { (7000, "Eisenstadt"), (7210, "Mattersburg"), (7100, "Neusiedl am See") } }
            };
            int counter = 1;
            var products = new Faker<Product>("de").CustomInstantiator(f =>
            {
                var minStock = f.Random.Int(0, 10);
                var availableFrom = f.Date.Between(
                    new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Date.OrNull(f, 0.5f);
                var availableTo = availableFrom is not null
                    ? f.Date.Between(availableFrom.Value, new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Date.OrNull(f, 0.5f)
                    : f.Date.Between(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Date.OrNull(f, 0.5f);
                return new Product(
                    Ean: f.Random.Int(100000, 999999).ToString(),
                    Name: f.Commerce.ProductName(), Category: f.Random.ListItem(categories),
                    RecommendedPrice: Math.Round(f.Random.Decimal(10, 999), 1),
                    Stock: f.Random.Bool(0.25f) ? (int)(f.Random.Double(0, 1) * minStock) : (int)(f.Random.Double(1, 10) * minStock),
                    MinStock: minStock,
                    AvailableFrom: availableFrom,
                    AvailableTo: availableTo,
                    Id: new ObjectId(counter++.ToString("x24")));
            })
            .Generate(10)
            .GroupBy(p => p.Ean).Select(g => g.First())
            .ToList();
            Products.InsertMany(products);
            Products.Indexes.CreateOne(
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(p => p.Ean),
                new CreateIndexOptions() { Unique = true }));

            counter = 1;
            var customers = new Faker<Customer>("de").CustomInstantiator(f =>
            {
                var lastname = f.Name.LastName();
                var orderItems = f.Random.ListItems(products, f.Random.Int(1, 4))
                    .Select(p => new OrderItem(Product: p, Quantity: f.Random.Int(1, 3), ItemPrice: p.RecommendedPrice))
                    .ToList();
                var addresses = new Faker<Address>("de").CustomInstantiator(f =>
                {
                    var state = f.Random.ListItem(states.Keys.ToList());
                    var city = f.Random.ListItem(states[state]);
                    return new Address(
                        Street: f.Address.StreetName(), StreetNr: f.Address.BuildingNumber(),
                        Zip: city.zip, City: city.name,
                        State: state);
                })
                .Generate(f.Random.Int(1, 3))
                .ToList();
                var customer = new Customer(
                    Name: new CustomerName(Firstname: f.Name.FirstName(), Lastname: lastname, Email: $"{lastname.ToLower()}@{f.Internet.DomainName()}"),
                    BillingAddress: addresses[0],
                    Id: new ObjectId(counter++.ToString("x24")));
                customer.ShippingAddresses.AddRange(addresses);
                return customer;
            })
            .Generate(10)
            .ToList();
            Customers.InsertMany(customers);

            counter = 1;
            var orders = new Faker<Order>("de").CustomInstantiator(f =>
            {
                var dateTime = new DateTime(f.Date.Between(new DateTime(2020, 1, 1), new DateTime(2022, 1, 1)).Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                var availableProducts = products
                    .Where(p =>
                        (p.AvailableFrom ?? DateTime.MinValue) <= dateTime.Date &&
                        (p.AvailableTo ?? DateTime.MaxValue) > dateTime.Date)
                    .ToList();
                var customer = f.Random.ListItem(customers);
                var orderItems = f.Random.ListItems(availableProducts, f.Random.Int(1, Math.Min(4, availableProducts.Count)))
                    .Select(p => new OrderItem(
                        Product: p,
                        Quantity: f.Random.Int(1, 3),
                        ItemPrice: f.Random.Bool(0.8f) ? p.RecommendedPrice : Math.Round(p.RecommendedPrice * f.Random.Decimal(0.90M, 0.99M), 1)))
                    .ToList();
                var order = new Order(
                    CustomerId: customer.Id,
                    CustomerName: customer.Name,
                    DateTime: new DateTime(2021, 1, 1).AddSeconds(f.Random.Int(0, 2 * 365 * 86_400)),
                    ShippingAddress: f.Random.ListItem(customer.ShippingAddresses),
                    Id: new ObjectId(counter++.ToString("x24")));
                order.OrderItems.AddRange(orderItems);
                return order;
            })
            .Generate(100)
            .ToList();
            Orders.InsertMany(orders);
        }
    }
}
