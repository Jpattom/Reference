using HA.Common;
using NServiceBus;
using System;

namespace HA.COSMOS.Messages
{
    [Serializable]
    public class DoJob : IBaseMessage, IMessage
    {

        public DoJob()
        {
            this.serviceParams = new object[0];
        }

        public DoJob(IBaseMessage message)
        {
            this.AssignFrom(message);
        }

        public void AssignFrom(IBaseMessage baseMessage)
        {
            this.serviceParams = baseMessage.GetServiceParams();
            this.UserContext = baseMessage.UserContext;
            this.ProcessContext = baseMessage.ProcessContext;
            this.SecurityToken = baseMessage.SecurityToken;
            this.ProcessId = baseMessage.ProcessContext.ProcessId;
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            // no need as of now
        }

        private object[] serviceParams;
        public object[] GetServiceParams()
        {
            return serviceParams;
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

        public Guid ProcessId { get; set; }
    }
}
