using HA.Contracts;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace HA.WCF.Proxies

{
    public class RACServiceProxy : DuplexClientBase<IService>, IService
    {
        public void InvokeService(IWCFMessage wcfMessage)
        {
            Console.WriteLine("At Rac service Proxy ");
            base.Channel.InvokeService(wcfMessage);
        }

        private RACServiceProxy(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        private RACServiceProxy(InstanceContext callbackInstance, string endpointConfigurationName)
            : base(callbackInstance, endpointConfigurationName)
        {
        }

        private RACServiceProxy(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        private RACServiceProxy(InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        private RACServiceProxy(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        public static RACServiceProxy InitializeClientProxy(string url)
        {
            try
            {
                IService racServiceProxy;
               ServiceProxyStore<string>.GetInstance().TryGet(url, out racServiceProxy);
                if (racServiceProxy != null)
                {
                    ((RACServiceProxy)racServiceProxy).Abort();
                    racServiceProxy = null;
                }

                RACCallBackProxy cb = new RACCallBackProxy();
                InstanceContext ctx = new InstanceContext(cb);
                racServiceProxy = new RACServiceProxy(ctx);
                ServiceProxyStore<string>.GetInstance().Remove(url);

                ServiceProxyStore<string>.GetInstance().Add(url, racServiceProxy);

                return (RACServiceProxy)racServiceProxy;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static RACServiceProxy InitializeClientProxy(string url, IService callbackproxy)
        {
            try
            {
                IService racServiceProxy;
                ServiceProxyStore<string>.GetInstance().TryGet(url, out racServiceProxy);
                if (racServiceProxy != null)
                {
                    ((RACServiceProxy)racServiceProxy).Abort();
                    racServiceProxy = null;
                }

                InstanceContext ctx = new InstanceContext(callbackproxy);
                racServiceProxy = new RACServiceProxy(ctx);
                ServiceProxyStore<string>.GetInstance().Remove(url);

                ServiceProxyStore<string>.GetInstance().Add(url, racServiceProxy);

                return (RACServiceProxy)racServiceProxy;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

   
}
