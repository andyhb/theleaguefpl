using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class FplDataRequest {
        [BsonId]
        public int Id;

        public DateTime RequestDate;
    }
}
