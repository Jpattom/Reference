using log4net;
using NServiceBus;
using NServiceBus.Hosting.Profiles;

using NServiceBus.Config;
using HA.Common;
using HA.COSMOS.Messages;
using HA.COSMOS.MessageHandlers;
using NServiceBus.Persistence;

namespace HA.COSMOS.Worker
{
    public class EndpointConfig : IConfigureThisEndpoint,  INeedInitialization
    {
        public void Init()
        {
           
        }



        public void Customize(BusConfiguration configuration)
        {
            configuration.UsePersistence<RavenDBPersistence>();
            configuration.UseSerialization(typeof(BinarySerializer));
        }
    }

    public class ServiceEndPoint : IWantToRunWhenBusStartsAndStops
    {
        public void Start()
        {
            ServiceBuilder serviceBuilder = ServiceBuilder.GetInstance();
            serviceBuilder.Initialize(typeof(HA.COSMOS.Services.UserServices), 
                typeof(HA.COSMOS.Mongo.DAL.UserDataAccessLayer));
        }

        public void Stop()
        {
           
        }
    }

    

}
