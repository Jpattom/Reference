﻿using System;
using NServiceBus.Saga;
using HA.COSMOS.Messages;

namespace HA.COSMOS.MessageHandlers
{
    public class EventSubscriptionSaga: Saga<EventSubscriptionSagaData>, IAmStartedByMessages<SubscribeEvents>
    {
        public void Handle(SubscribeEvents message)
        {
          
        }
    }
}
