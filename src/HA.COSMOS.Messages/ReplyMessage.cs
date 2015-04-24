using NServiceBus;
using HA.Common;
using HA.COSMOS.ValueObjects;
using System;

namespace HA.COSMOS.Messages
{
    [Serializable]
    public class ReplyMessage : IBaseMessage, IMessage
    {
        public ReplyMessage()
        {
            ProcessContext = new ProcessContext();
        }

        public ReplyMessage(params object[] seerviceprams)
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

        public ReplyMessage(object[] serviceParams, string securityToken, IUserContext userContext, IProcessContext processContext)
        {
            this.serviceParams = serviceParams;
            this.SecurityToken = securityToken;
            this.UserContext = userContext;
            this.ProcessContext = processContext;
        }

        private object[] serviceParams;

        public object[] GetServiceParams()
        {
            return serviceParams;
        }

        public void SetServiceParams(params object[] sp)
        {
            serviceParams = sp;
        }

        public string SecurityToken { get; set; }

        public IUserContext UserContext { get; set; }

        public IProcessContext ProcessContext
        {
            get;
            set;
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
    }
}
