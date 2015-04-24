using HA.Common;
using System;

namespace HA.Contracts
{
    public interface IServiceRequestProcessor
    {
        void ProcessServiceRequest(IBaseMessage request);
        void ReplyServiceRequest(IBaseMessage reply);
    }

    public class ServiceRequestProcessorProvider
    {
        private object locObject = new object();
        private ServiceRequestProcessorProvider()
        {
            serviceRequestProcessor = null;
        }

        private static ServiceRequestProcessorProvider instance;
        public static ServiceRequestProcessorProvider GetInstance()
        {
            //lock (locObject)
            //{
                if (instance == null)
                    instance = new ServiceRequestProcessorProvider();
                return instance;
            //}

        }
        private IServiceRequestProcessor serviceRequestProcessor;
        public IServiceRequestProcessor ServiceRequestProcessor
        {
            get
            {
                return serviceRequestProcessor;
            }
            set
            {
                if (serviceRequestProcessor != null)
                    throw new Exception("Service Request Processor is already set");
                serviceRequestProcessor = value;
            }
        }

    }

}
