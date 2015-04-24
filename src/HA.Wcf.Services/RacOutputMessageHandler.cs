using NServiceBus;
using HA.Contracts;
using HA.NServiceBus.Messages;
using HA.Wcf.Messages;
using System;

namespace HA.Wcf.Services
{
    public class RacOutputMessageHandler : IHandleMessages<RacOutputMessage>
    {

        public void Handle(RacOutputMessage message)
        {
            CallBackContextStore store = CallBackContextStore.GetInstance();
            IRACCallBack callbackContext = store.Get(message.ProcessId);
            callbackContext.InvokeService(new WCFMessage(new object[]{string.Format("Reply for {}", message.ProcessId)}));

        }
    }
}
