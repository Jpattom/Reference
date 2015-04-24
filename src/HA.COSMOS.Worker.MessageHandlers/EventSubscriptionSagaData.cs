using System;
using NServiceBus.Saga;

namespace HA.COSMOS.MessageHandlers
{
    public class EventSubscriptionSagaData: IContainSagaData
    {
        public Guid Id
        {
            get;
            set;
        }

        public string OriginalMessageId
        {
            get;
            set;
        }

        public string Originator
        {
            get;
            set;
        }
    }
}
