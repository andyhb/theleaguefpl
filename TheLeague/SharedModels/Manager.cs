using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Manager {
        [BsonId]
        public int Id;

        public string Name;

        public Manager() { }

        public Manager(int id, string name) {
            Id = id;
            Name = name;
        }
    }
}
