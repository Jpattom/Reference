

using HA.Common;
using HA.Contracts;
using HA.COSMOS.ValueObjects;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using NServiceBus;
using NServiceBus.Features;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Client, INeedInitialization
    {

        public void Customize(BusConfiguration configuration)
        {
            configuration.PurgeOnStartup(true);
            configuration.Transactions().Disable();
            configuration.DisableFeature<SecondLevelRetries>();
            configuration.DisableFeature<StorageDrivenPublishing>();
            configuration.DisableFeature<TimeoutManager>();
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseSerialization(typeof(BinarySerializer));
        }
    }


    public class ServiceEndPoint : IWantToRunWhenBusStartsAndStops, IServiceRequestProcessor
    {

        public static ServiceEndPoint MyInstance { get; private set; }

        public IBus Bus { get; set; }

        private HttpSelfHostServer server;

        IDisposable webApp;

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

                string url = "http://localhost:5053";
                webApp = WebApp.Start(url);


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
            if (webApp != null)
                webApp.Dispose();
            webApp = null;

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

            var clients = GlobalHost.ConnectionManager.GetHubContext<WebApiPersistentConnection>();
            //clients.Clients.All.addMessage("Jobstatus", reply.ProcessContext.ProcessId.ToString());


            if (clients != null)
            {
                clients.Clients.Client(reply.ProcessContext.ProcessId.ToString()).addMessage(reply.ProcessContext.ProcessId.ToString(), reply.ProcessContext.ProcessId.ToString());

            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }


    public class WebApiPersistentConnection : Hub
    {
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
            //Clients.Client()
        }
    }
}
