using HA.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace HA.WCF.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BaseService : IService, IDisposable
    {
        public void InvokeService(IWCFMessage wcfMessage)
        {
            var operationContext = OperationContext.Current;
            var processId = wcfMessage.ProcessContext.ProcessId;
            var processType = wcfMessage.ProcessContext.ProcessType;
            var securityToken = wcfMessage.SecurityToken;

            RemoteEndpointMessageProperty endpoint = null;

            if (operationContext != null)
            {
                var properties = operationContext.IncomingMessageProperties;
                endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                System.Diagnostics.Debug.WriteLine(string.Format("InvokeRACService start for ProcessType:{0} with ProcessId:{1}, Connected from {2}:{3} ",
                processType, processId, endpoint.Address, endpoint.Port));
                var callbackContext = operationContext.GetCallbackChannel<IService>();
                if (callbackContext != null)
                {
                    var store = ServiceProxyStore<Guid>.GetInstance();
                    store.Add(processId, callbackContext);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("No client data available, InvokeRACService start for ProcessType:{0} with ProcessId:{1}",
                    processType, processId));
            }
            
            var racServiceRequestProcessorProvider = ServiceRequestProcessorProvider.GetInstance();
            if (racServiceRequestProcessorProvider.ServiceRequestProcessor != null)
            {
                racServiceRequestProcessorProvider.ServiceRequestProcessor.ProcessServiceRequest(wcfMessage);
            }
        }

        public void Dispose()
        {
           
        }
    }
}
