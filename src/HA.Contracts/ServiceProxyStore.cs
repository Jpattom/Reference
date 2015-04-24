using System;
using System.Collections.Generic;

namespace HA.Contracts
{

    public interface IServiceProxyStore<in T>
    {
        void Add(T key, IService value);
        IService Get(T key);
        void TryGet(T key, out IService racService);
        void Remove(T key);
        bool ContainsKey(T key);
    }

    public class ServiceProxyStore<T>: IServiceProxyStore<T>
    {
        private Dictionary<T, IService> callBackContexts;

        private static ServiceProxyStore<T> instance;

        private ServiceProxyStore()
        {
            callBackContexts = new Dictionary<T, IService>();
        }

        public static ServiceProxyStore<T> GetInstance()
        {
            if (instance == null)
                instance = new ServiceProxyStore<T>();
            return instance;
        }

        public void Add(T key, IService value)
        {
            if (callBackContexts.ContainsKey(key))
                callBackContexts.Remove(key);
            callBackContexts.Add(key, value);
        }

        public IService Get(T key)
        {
            if (callBackContexts.ContainsKey(key))
                return callBackContexts[key];
            else
                throw new Exception("Could not find in the store");
        }

        public void TryGet(T key, out IService racService)
        {
            racService = null;
            if (callBackContexts.ContainsKey(key))
                racService = callBackContexts[key];

        }

        public void Remove(T key)
        {
            if (callBackContexts.ContainsKey(key))
                callBackContexts.Remove(key);
        }



        public bool ContainsKey(T key)
        {
            return callBackContexts.ContainsKey(key);
        }
    }
}
