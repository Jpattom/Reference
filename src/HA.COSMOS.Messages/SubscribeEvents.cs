using NServiceBus;
using HA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.COSMOS.Messages
{

    [Serializable]
    public class SubscribeEvents : IBaseMessage, ICommand
    {
        private object[] serviceParams;

        public object[] GetServiceParams()
        {
            return serviceParams;
        }

         public SubscribeEvents()
        {
            ProcessContext = new ProcessContext();
        }

        public SubscribeEvents(params object[] seerviceprams)
        {
            this.ProcessContext = new ProcessContext();
            if (serviceParams != null && serviceParams.Length > 0)
            {
                this.serviceParams = new object[serviceParams.Length];
                for (int i = 0; i < serviceParams.Length; i++)
                {
                    this.serviceParams[i] = serviceParams[i];
                }
            }
        }

        public SubscribeEvents(object[] serviceParams, string securityToken, IUserContext userContext, IProcessContext processContext)
        {
            this.serviceParams = serviceParams;
            this.SecurityToken = securityToken;
            this.UserContext = userContext;
            this.ProcessContext = ProcessContext;
        }

        public void AssignFrom(IBaseMessage baseMessage)
        {
            ProcessContextUtil.AssignFrom(baseMessage.ProcessContext, this.ProcessContext);
            this.SecurityToken = baseMessage.SecurityToken;
            this.UserContext = baseMessage.UserContext;
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            ProcessContextUtil.AssignTo(baseMessage.ProcessContext, this.ProcessContext);
            baseMessage.UserContext = this.UserContext;
            baseMessage.SecurityToken = this.SecurityToken;
        }

        public IProcessContext ProcessContext
        {
            get;
            set;
        }

        public string SecurityToken
        {
            get;
            set;
        }

        public IUserContext UserContext
        {
            get;
            set;
        }
    }
}
