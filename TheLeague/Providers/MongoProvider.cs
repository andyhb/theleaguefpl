using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TheLeague.Providers {
    public class MongoProvider {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public const string Uri = "configure this yourselves";
        public const string Database = "and this";
    }
}
