using NServiceBus;
using HA.Contracts;
using HA.COSMOS.Messages;
using System;

namespace HA.COSMOS.Service.MessageHandlers
{
    public class ReplyMessageHandler: IHandleMessages<ReplyMessage>
    {
        public void Handle(ReplyMessage message)
        {
            ServiceRequestProcessorProvider serviceRequestProcessorProvider = ServiceRequestProcessorProvider.GetInstance();
            if (serviceRequestProcessorProvider.ServiceRequestProcessor != null)
            {
                serviceRequestProcessorProvider.ServiceRequestProcessor.ReplyServiceRequest(message);
            }
        }
    }
}
