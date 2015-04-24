using HA.Common;
using NServiceBus;
using System;

namespace HA.COSMOS.Messages
{
     [Serializable]
    public class EndSaga1: IBaseMessage,ICommand
    {
         public EndSaga1()
         {
         }

         public EndSaga1(IBaseMessage baseMessage)
         {
             this.AssignFrom(baseMessage);
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
