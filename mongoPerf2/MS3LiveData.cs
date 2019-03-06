using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;



namespace mongoPerf2
{
    

    public class MS3LiveData
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("modeName")]
        public string ModeName { get; set; }
        [BsonElement("jobName")]
        public string JobName { get; set; }
        [BsonElement("jobId")]
        public string JobId { get; set; }
        [BsonElement("data")]
        public  List<BsonDocument> Data { get; set; }
        [BsonElement("count")]
        public long Count { get; set; }
        [BsonElement("date")]
        public DateTime Date { get; set; }



        public MS3LiveData(string modeName, string jobName, string jobId, List<BsonDocument> data, long count, DateTime date)
        {
            ModeName = modeName;
            JobName = jobName;
            JobId = jobId;
            Data = data;
            Count = count;
            Date = date;
        }

      





    }



}

