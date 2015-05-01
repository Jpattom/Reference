using NServiceBus;
using HA.Common;
using HA.Contracts;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using HA.WCF.Messages;
using HA.WCF.Services;
using System;
using System.ServiceModel;
using NServiceBus.Features;

namespace HA.WCF.COSMOS
{
    /// <summary>
    /// Class Responsible for COSMOS Wcf end point configuaration
    /// </summary>
    public class EndpointConfig : IConfigureThisEndpoint, INeedInitialization
    {
        public void Init()
        {

        }

        public void Customize(BusConfiguration configuration)
        {
            configuration.PurgeOnStartup(true);
            configuration.Transactions().Disable();
            configuration.DisableFeature<SecondLevelRetries>();
            configuration.DisableFeature<StorageDrivenPublishing>();
            configuration.DisableFeature<TimeoutManager>();
            configuration.UseSerialization(typeof(BinarySerializer));
        }
    }

    /// <summary>
    /// Class Responsible for COSMOS Wcf end point processing 
    /// </summary>
    public class ServiceEndPoint : IWantToRunWhenBusStartsAndStops, IServiceRequestProcessor
    {
        private System.ServiceModel.ServiceHost svcHost;

        public IBus Bus { get; set; }

        public void Start()
        {
            try
            {
                svcHost = new ServiceHost(typeof(BaseService));
                svcHost.Open();
                Console.WriteLine("RAC service is running... ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("Press Enter to Quit");
                Console.ReadLine();
                svcHost.Close();
            }
        }

        public void Stop()
        {
            Console.WriteLine("Stoping....");
        }

        public void ProcessServiceRequest(IBaseMessage request)
        {
            Console.WriteLine(string.Format("ProcessServiceRequest {0}", request.ProcessContext.ProcessType));
            IBaseMessage workRequest = null;
            switch (request.ProcessContext.ProcessType)
            {
                case ProcessTypes.Login:
                    object[] serviceParams = request.GetServiceParams();
                    workRequest = new Login((LoginVO)serviceParams[0]);
                    break;
                case ProcessTypes.GetAllAppUsers:
                    workRequest = new GetAllAppUsers();
                    break;
            }
            if (workRequest != null && workRequest is IMessage)
            {
                workRequest.AssignFrom(request);
                ErrorCodeHandler erroCodeHandler = new ErrorCodeHandler(request.ProcessContext);
                Bus.Send(workRequest).Register(erroCodeHandler.HandleErrorCode, erroCodeHandler);
            }
        }

        public void ReplyServiceRequest(IBaseMessage reply)
        {
            ServiceProxyStore<Guid> callBackContextStore = ServiceProxyStore<Guid>.GetInstance();
            IService racCallBackContext = callBackContextStore.Get(reply.ProcessContext.ProcessId);
            WCFMessage replyMessage = new WCFMessage(reply.GetServiceParams());
            replyMessage.AssignFrom(reply);
            racCallBackContext.InvokeService(replyMessage);
            if (reply.ProcessContext.IsLastMessage)
            {
                callBackContextStore.Remove(reply.ProcessContext.ProcessId);
            }
        }
    }

    /// <summary>
    ///Class responsible for handling error code returned from worker process
    /// </summary>
    internal class ErrorCodeHandler
    {
        private IProcessContext processContext;

        internal ErrorCodeHandler(IProcessContext processContext)
        {
            this.processContext = processContext;
        }

        internal void HandleErrorCode(IAsyncResult asyncResult)
        {
            ServiceProxyStore<Guid> callBackContextStore = ServiceProxyStore<Guid>.GetInstance();
            IService callBackContext = callBackContextStore.Get(processContext.ProcessId);
            var result = asyncResult.AsyncState as CompletionResult;
            var replyMessage = new WCFMessage(result.ErrorCode);
            replyMessage.ProcessContext = this.processContext;
            callBackContext.InvokeService(replyMessage);
        }
    }
}
