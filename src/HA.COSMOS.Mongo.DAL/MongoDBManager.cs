using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using HA.COSMOS.Entities;
using System;


namespace HA.COSMOS.Mongo.DAL
{
    internal class MongoDBManager : IDisposable
    {
        private MongoDatabase rmacDataBase;
        private MongoDBManager()
        {
            InitiateDataBase();
        }

        private static MongoDBManager instance;

        internal static MongoDBManager GetInstane()
        {
            if (instance == null)
                instance = new MongoDBManager();

            return instance;
        }

        public MongoDatabase COSMOSDataBase
        {
            get
            {
                if (rmacDataBase == null)
                {
                    InitiateDataBase();
                }
                return rmacDataBase;
            }
        }

        private void InitiateDataBase()
        {
#warning Hard coding need refactoring
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            rmacDataBase = server.GetDatabase("rmac");
            ConfigureBSONClassMapp();
        }

        private void ConfigureBSONClassMapp()
        {
            
#warning kind of hard coding need refactoring to add types entites dynamicaly 

            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(classMap =>
                    {
                        classMap.AutoMap();
                        classMap.SetIdMember(classMap.GetMemberMap(user => user.UserName));
                    });
            }
        }

        public void Dispose()
        {

        }
    }
}