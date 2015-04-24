using NServiceBus;
using HA.Common;
using HA.COSMOS.ValueObjects;
using System;

namespace HA.COSMOS.Messages
{
    public class GetDataForCache : IBaseMessage, ICommand
    {

        public GetDataForCache()
        {
            ProcessContext = new ProcessContext();
        }

        public CacheableEntities[] CacheableEntities
        {
            get;
            set;
        }

        public GetDataForCache(params CacheableEntities[] cacheableEntities)
        {
            ProcessContext = new ProcessContext();
            if (cacheableEntities != null)
            {
                serviceParams = new object[cacheableEntities.Length];
                for (int i = 0; i < cacheableEntities.Length; i++)
                {
                    serviceParams[i] = (object)cacheableEntities[i];
                }
                CacheableEntities = cacheableEntities;
            }

        }

        public GetDataForCache(CacheableEntities[] cacheableEntities, IProcessContext processContext, IUserContext userContext, string securityToken)
        {
            ProcessContext = processContext;
            SecurityToken = securityToken;
            UserContext = userContext;
            if (cacheableEntities != null)
            {
                serviceParams = new object[cacheableEntities.Length];
                for (int i = 0; i < cacheableEntities.Length; i++)
                {
                    serviceParams[i] = (object)cacheableEntities[i];
                }
                CacheableEntities = cacheableEntities;

            }

        }

        public string SecurityToken { get; set; }

        private object[] serviceParams;
        public object[] GetServiceParams()
        {
            return serviceParams;
        }

        public IProcessContext ProcessContext { get; set; }

        public void AssignFrom(IBaseMessage baseMessage)
        {
            this.SecurityToken = baseMessage.SecurityToken;
            this.ProcessContext = baseMessage.ProcessContext;
            this.UserContext = baseMessage.UserContext;
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            baseMessage.SecurityToken = this.SecurityToken;
            //baseMessage.ProcessContext = baseMessage.ProcessContext;
            baseMessage.UserContext = this.UserContext;
        }

        public IUserContext UserContext { get; set; }
    }
}
