using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using System.Security.Cryptography;
using System.Text;


namespace mongoPerf2
{
    class MainClass
    {

        //static MongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
        static MongoClient client = new MongoClient("mongodb://amsted34:pinttw68@mongotest-shard-00-00-fxdpg.azure.mongodb.net:27017,mongotest-shard-00-01-fxdpg.azure.mongodb.net:27017,mongotest-shard-00-02-fxdpg.azure.mongodb.net:27017/test?ssl=true&replicaSet=Mongotest-shard-0&authSource=admin&retryWrites=true");
        static IMongoDatabase db = client.GetDatabase("MS3LiveDataDB");
        static IMongoCollection<MS3LiveData> MS3Collection = db.GetCollection<MS3LiveData>("DATA");


        public static async Task Main(string[] args)
        {
            var count = 0; 
            WriteConcern WCValue = new WriteConcern(1);
            client.WithWriteConcern(WCValue);
            string modeName = "Mode 3 Cams";
            string jobName = "CA20190305";
            string jobId = "SANTANDER1000"; 
            List<BsonDocument> data = new List<BsonDocument>
            {
                new BsonDocument("OCR1", GetUniqueKey(15)),
                new BsonDocument("OCR2", GetUniqueKey(15)),
                new BsonDocument("OCR3", GetUniqueKey(15)),
                new BsonDocument("OCR4", GetUniqueKey(15))
            };

            //List<BsonDocument> data = new List<BsonDocument>(new List<BsonDocument>( ReadData));
          
            Console.WriteLine("Program Start");
            var docCount = MS3Collection.CountDocuments(new BsonDocument());
           

            MS3Collection.InsertOne(new MS3LiveData(modeName, jobName, jobId, data, docCount++, DateTime.Now));
            MS3Collection.Indexes.CreateOne(new BsonDocument("modeName", 1));
            MS3Collection.Indexes.CreateOne(new BsonDocument("jobName", 1));
            MS3Collection.Indexes.CreateOne(new BsonDocument("data", 1));


            for (int i = 0; i<1000000; i++)
            {
                Stopwatch st1 = new Stopwatch();
                st1.Start();
                data = new List<BsonDocument>
            {
                new BsonDocument("OCR1", GetUniqueKey(15)),
                new BsonDocument("OCR2", GetUniqueKey(15)),
                new BsonDocument("OCR3", GetUniqueKey(15)),
                new BsonDocument("OCR4", GetUniqueKey(15))
            };
                MS3LiveData MS3Data = new MS3LiveData(modeName, jobName, jobId, data, docCount++, DateTime.Now);
                count++;
                await CreateOne(MS3Data);

                Console.WriteLine($"i:{i} elapsed: {st1.Elapsed.TotalMilliseconds}");
                st1.Stop(); 
            }



          // ReadAll(); 
        }


        public static async Task<string> CreateOne(MS3LiveData mS3LiveDataCollection)
        {
            await MS3Collection.InsertOneAsync(mS3LiveDataCollection);
            return "inserted"; 
          
        }

        public static void ReadAll()
        {
            Stopwatch st2 = new Stopwatch();

            double previous = 0; 
            List<MS3LiveData> list = MS3Collection.AsQueryable().ToList<MS3LiveData>(); 
            //foreach (MS3LiveData l in list) { Console.WriteLine("Read" + l.Id); }
            var myData = from s in list select s;

            st2.Start();
            Console.WriteLine("the current document/s:");
             foreach(MS3LiveData m in myData)
            {
                foreach( BsonDocument data in m.Data) {

                    Console.WriteLine(m.ToJson());
                }
                Console.WriteLine(st2.Elapsed.TotalMilliseconds - previous);
                previous = st2.Elapsed.TotalMilliseconds; 
            }

          
           list.Clear();
           

            Console.WriteLine(st2.Elapsed.TotalMilliseconds );
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine("garbage collection ended");

        }

        public static string GetUniqueKey(int size)
        {
            char[] chars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

    }
}
