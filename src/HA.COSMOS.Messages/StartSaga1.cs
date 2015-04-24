using HA.Common;
using NServiceBus;
using System;

namespace HA.COSMOS.Messages
{
    [Serializable]
    public class StartSaga1 : IBaseMessage, ICommand
    {
        public StartSaga1()
        {
            this.serviceParams = new object[0];
        }


        public StartSaga1(object[] serviceParams)
        {
            this.serviceParams = serviceParams;
        }


        public void AssignFrom(IBaseMessage baseMessage)
        {
            this.serviceParams = baseMessage.GetServiceParams();
            this.UserContext = baseMessage.UserContext;
            this.ProcessContext = baseMessage.ProcessContext;
            this.SecurityToken = baseMessage.SecurityToken;
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
        }

        private object[] serviceParams;
        public object[] GetServiceParams()
        {
            return serviceParams;
        }

        public object[] Serviceparams
        {
            get
            {
                return serviceParams;
            }
            set
            {
                serviceParams = value;
            }
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
