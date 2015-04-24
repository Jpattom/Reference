

using HA.Common;
using HA.Contracts;
using HA.COSMOS.ValueObjects;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace HA.COSMOS.WebApi
{
    public sealed class ServiceResultStore
    {
        private static ServiceResultStore myInstance;

        private Dictionary<string, ServiceMessage> results;

        private ServiceResultStore()
        {
            results = new Dictionary<string, ServiceMessage>();

        }

        public void Add(string processId, ServiceMessage messaage)
        {
            if (results.ContainsKey(processId))
            {
                results[processId] = messaage;
            }
            else
            {
                results.Add(processId, messaage);
            }
        }

        public void Remove(string processId)
        {
            if (results.ContainsKey(processId))
            {
                results.Remove(processId);
            }

        }

        public ServiceMessage Get(string processId)
        {
            if (results.ContainsKey(processId))
                return results[processId];
            return null;
        }

        public static ServiceResultStore Instance { get { return Nested.ServiceResultStore; } }

        private class Nested
        {
            static Nested() { }
            internal static readonly ServiceResultStore ServiceResultStore = new ServiceResultStore();

        }
    }


    /// <summary>
    /// Class Responsible for COSMOS WebApi end point configuaration
    /// </summary>
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Client, IWantCustomInitialization
    {
        public void Init()
        {
            NServiceBus.Configure.With()
                .DefaultBuilder()
                .BinarySerializer();
        }
    }


    public class ServiceEndPoint : IWantToRunWhenBusStartsAndStops, IServiceRequestProcessor
    {

        public static ServiceEndPoint MyInstance { get; private set; }

        public IBus Bus { get; set; }

        private HttpSelfHostServer server;

        public void Start()
        {
            try
            {
                ServiceEndPoint.MyInstance = this;
                ServiceRequestProcessorProvider.GetInstance().ServiceRequestProcessor = this;

                var config = new HttpSelfHostConfiguration("http://localhost:5052");
                config.Routes.MapHttpRoute(
                    "API Default", "api/{controller}/{id}",
                    new { id = RouteParameter.Optional });

                server = new HttpSelfHostServer(config);

                server.OpenAsync().Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {

            }
        }

        public void Stop()
        {

            if (server != null)
            {
                server.CloseAsync();
                server = null;
            }
            ServiceEndPoint.MyInstance = null;
        }

        public void ProcessServiceRequest(IBaseMessage request)
        {
            throw new NotImplementedException();
        }

        public void ReplyServiceRequest(IBaseMessage reply)
        {
            var serviceResultStore = ServiceResultStore.Instance;
            serviceResultStore.Add(reply.ProcessContext.ProcessId.ToString(), new ServiceMessage
            {
                ErrorCode = 0,
                ProcessContext = (ProcessContext)reply.ProcessContext,
                SecurityToken = reply.SecurityToken,
                ServiceParams = reply.GetServiceParams(),
                UserContext = (COSMOSUSerContext)reply.UserContext
            });
        }
    }
}
