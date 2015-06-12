using log4net;
using NServiceBus;
using NServiceBus.Hosting.Profiles;

using NServiceBus.Config;
using HA.Common;
using HA.COSMOS.Messages;
using HA.COSMOS.MessageHandlers;
using NServiceBus.Persistence;
using System;

namespace HA.COSMOS.Worker
{

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        public void Customize(BusConfiguration configuration)
        {
            configuration.UsePersistence<RavenDBPersistence>();
            configuration.UseSerialization(typeof(BinarySerializer));
            configuration.SecondLevelRetries()
                .CustomRetryPolicy(GetCustomRetryPolicy);
            configuration.PurgeOnStartup(true);
            

        }

        TimeSpan GetCustomRetryPolicy(TransportMessage message)
        {
            // retry max 3 times
            if (GetNumberOfRetries(message) >= 3)
            {
                // sending back a TimeSpan.MinValue tells the 
                // SecondLevelRetry not to retry this message
                return TimeSpan.MinValue;
            }

            return TimeSpan.FromSeconds(10);
        }

        static int GetNumberOfRetries(TransportMessage message)
        {
            string value;
            Console.WriteLine("Message Type:{0}", message.Headers[Headers.EnclosedMessageTypes]);

            if (message.Headers.TryGetValue(Headers.Retries, out value))
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    Console.WriteLine("No of retries for Message Type:{0} is {1}", message.Headers[Headers.ContentType], value);
                    return i;

                }
            }
            return 0;
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
