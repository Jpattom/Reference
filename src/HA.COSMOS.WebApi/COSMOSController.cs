using HA.Contracts;
using HA.WCF.Messages;
using HA.Common;

using System;
using System.Web.Http;
using HA.COSMOS.ValueObjects;
using HA.COSMOS.Messages;
using NServiceBus;

namespace HA.COSMOS.WebApi
{
    public class COSMOSController : ApiController
    {

        [HttpPost]
        public int InvokeService(dynamic message)
        {
            using (HA.WCF.Services.BaseService service = new WCF.Services.BaseService())
            {
                Newtonsoft.Json.Linq.JObject jObject = message;
                var serviceMessage = jObject.ToObject<ServiceMessage>();
                var requestMessage = MessageFactory.GetFineGrainedMessage(serviceMessage);
                var wcfMessage = new WCFMessage(serviceMessage.ErrorCode, requestMessage.GetServiceParams(), serviceMessage.ProcessContext, serviceMessage.SecurityToken, serviceMessage.UserContext);
                requestMessage.AssignFrom(wcfMessage);
                Console.WriteLine(string.Format("Process Service Request for {0}", requestMessage.ProcessContext.ProcessType));
                var nsbResult = new NServiceBus.CompletionResult();
                var asyncResult = ServiceEndPoint.MyInstance.Bus.Send((IMessage)requestMessage).Register(code => nsbResult = code.AsyncState as NServiceBus.CompletionResult, null);
                asyncResult.AsyncWaitHandle.WaitOne(100000);
                Console.WriteLine(nsbResult.ErrorCode);
                return nsbResult.ErrorCode;
            }
        }

        [HttpGet]
        public dynamic GetServiceResult([FromUri]string processId)
        {
            string procId = processId;
            Console.WriteLine(procId);
            var serviceMessage = ServiceResultStore.Instance.Get(procId);
            if(serviceMessage != null)
                ServiceResultStore.Instance.Remove(procId);
            return serviceMessage;
        }
    }

    public class MessageFactory
    {

        public static IBaseMessage GetFineGrainedMessage(ServiceMessage request)
        {
            
            IBaseMessage workRequest = null;
            switch (request.ProcessContext.ProcessType)
            {
                case ProcessTypes.Login:

                    object[] serviceParams = new object[request.ServiceParams.Length];
                    int i = 0;
                    foreach (Newtonsoft.Json.Linq.JObject obj in request.ServiceParams)
                    {
                        serviceParams[i] = obj.ToObject<LoginVO>();
                        i++;
                    }
                    workRequest = new Login((LoginVO)serviceParams[0]);
                    break;
                case ProcessTypes.GetAllAppUsers:
                    workRequest = new GetAllAppUsers();
                    break;
                case ProcessTypes.DoJob:
                    object[] serviceParams1 = new object[request.ServiceParams.Length];
                    int j = 0;
                    foreach (string obj in request.ServiceParams)
                    {
                        serviceParams1[j] = obj;
                        j++;
                    }
                    workRequest = new StartSaga1(serviceParams1);
                    break;
            }
            return workRequest;
        }
    }
}
