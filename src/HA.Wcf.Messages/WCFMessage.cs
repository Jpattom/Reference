using System;
using HA.Common;
using HA.Contracts;
using Newtonsoft.Json;

namespace HA.WCF.Messages
{
    [Serializable]
    [JsonObject]
    public class WCFMessage : IWCFMessage
    {
        public WCFMessage(params object[] serviceParams)
        {
            ErrorCode = 0;
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

        public WCFMessage(int errorCode, params object[] serviceParams)
        {
            ErrorCode = errorCode;
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

        public WCFMessage(int errorCode) 
        {
            this.ErrorCode = errorCode; 
        }

        [JsonConstructor]
        public WCFMessage(int errorCode, object[] serviceParams, IProcessContext processContext, string securityToken, IUserContext userContext)
        {
            this.ErrorCode = errorCode;
            this.ProcessContext = processContext;
            this.UserContext = userContext;
            this.SecurityToken = securityToken;
            if (serviceParams != null && serviceParams.Length > 0)
            {
                this.serviceParams = new object[serviceParams.Length];
                for (int i = 0; i < serviceParams.Length; i++)
                {
                    this.serviceParams[i] = serviceParams[i];
                }
            }
        }

        public int ErrorCode { get; private set; }

        private object[] serviceParams;

        public object[] GetServiceParams()
        {
            return serviceParams;
        }

        public object[] ServiceParams
        {
            get
            {
                return serviceParams;
            }

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
            baseMessage.SecurityToken = this.SecurityToken;
            baseMessage.UserContext = this.UserContext;
        }
    }
}
