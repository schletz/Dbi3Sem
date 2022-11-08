using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ICSharpCode.SharpZipLib.BZip2;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

public record Measurement(
    long Id, int Station, DateTime Date, DateTime Datetime, int Year, int Month, int Day, int Hour, int Minute,
    double? Temp, double? Dewp, double? Pressure, double? Prec_amount, double? Prec_duration, double? Cloud_octas,
    double? Wind_dir, double? Wind_speed, double? Max_temp, double? Min_temp, double? Sunshine);

internal class Program
{
    private static void Main(string[] args)
    {
        var measurements = ReadMeasurements("measurements.txt.bz2");
        var db = GetDatabase();
        Console.WriteLine("Some tests WITHOUT index...");
        db.DropCollection("measurements");
        ImportData(db, measurements);
        GetStationData(db, 11082);
        GetTempBelow(db, -10);

        Console.WriteLine("Some tests WITH index...");
        db.DropCollection("measurements");
        //db.GetCollection<Measurement>("measurements").Indexes.CreateOne(
        //    new CreateIndexModel<Measurement>(Builders<Measurement>.IndexKeys.Ascending(m => m.Station)));
        //db.GetCollection<Measurement>("measurements").Indexes.CreateOne(
        //    new CreateIndexModel<Measurement>(Builders<Measurement>.IndexKeys.Ascending(m => m.Temp)));
        ImportData(db, measurements);
        GetStationData(db, 11082);
        GetTempBelow(db, -10);
    }

    private static IMongoDatabase GetDatabase()
    {
        var conventions = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreIfNullConvention(ignoreIfNull: true)
        };
        ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventions, _ => true);
        var settings = MongoClientSettings.FromConnectionString("mongodb://root:1234@localhost:27017");
        var client = new MongoClient(settings);
        return client.GetDatabase("weatherdata");
    }
    /// <summary>
    /// Liest die Messwerte aus einer .txt.bz2 Datei und liefert sie als Liste zurück.
    /// </summary>
    private static List<Measurement> ReadMeasurements(string filename)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };
        using var bzStream = new BZip2InputStream(File.OpenRead("measurements.txt.bz2"));
        using var reader = new StreamReader(bzStream, new UTF8Encoding(false));
        using var csv = new CsvReader(reader, csvConfig);
        return csv.GetRecords<Measurement>().ToList();
    }

    public static void ImportData(IMongoDatabase db, List<Measurement> measurements)
    {
        var sw = new Stopwatch();
        GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true);
        sw.Start();
        db.GetCollection<Measurement>("measurements").InsertMany(measurements);
        sw.Stop();
        Console.WriteLine($"Inserted {measurements.Count} Records in {sw.ElapsedMilliseconds} ms");
    }
    public static List<Measurement> GetStationData(IMongoDatabase db, int stationId)
    {
        var sw = new Stopwatch();
        GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true);
        sw.Start();
        var result = db.GetCollection<Measurement>("measurements").Find(Builders<Measurement>.Filter.Eq(m => m.Station, stationId)).ToList();
        sw.Stop();
        Console.WriteLine($"GetStationData: {result.Count} records read in {sw.ElapsedMilliseconds} ms");
        return result;
    }
    public static List<Measurement> GetTempBelow(IMongoDatabase db, double temp)
    {
        var sw = new Stopwatch();
        GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true);
        sw.Start();
        var result = db.GetCollection<Measurement>("measurements").Find(Builders<Measurement>.Filter.Lt(m => m.Temp, temp)).ToList();
        sw.Stop();
        Console.WriteLine($"GetTempBelow {temp}°: {result.Count} records read in {sw.ElapsedMilliseconds} ms");
        return result;
    }
}