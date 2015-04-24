using log4net;
using NServiceBus;
using NServiceBus.Config;
using HA.Common;
using HA.COSMOS.Messages;
using HA.COSMOS.MessageHandlers;

namespace HA.COSMOS.Worker
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization//, ISpecifyMessageHandlerOrdering
    {
        public void Init()
        {
            ILog logger = LogManager.GetLogger(typeof(EndpointConfig));
            logger.Info("Worker Getting initilized");
            Configure.With()
                .DefaultBuilder()
                .BinarySerializer();
        }

        //public void SpecifyOrder(Order order)
        //{
        //    order.Specify(typeof(DoJob4), typeof(DoJob2), typeof(DoJob3), typeof(DoJob1));
        //}
    }

    public class ServiceEndPoint : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }
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
