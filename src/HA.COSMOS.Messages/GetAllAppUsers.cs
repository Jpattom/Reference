
using NServiceBus;
using HA.Common;
using System;
namespace HA.COSMOS.Messages
{
    [Serializable]
    public class GetAllAppUsers: IBaseMessage, ICommand
    {
        #region Constructors
        public GetAllAppUsers()
        {
            ProcessContext = new ProcessContext();
            serviceParams = new object[0];
        }

        public GetAllAppUsers(IProcessContext processContext, IUserContext userContext)
        {
            ProcessContext = processContext;
            UserContext = userContext;
            serviceParams = new object[0];
        }

        #endregion


        public string SecurityToken
        {
            get;
            set;
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

        public IUserContext UserContext
        {
            get;
            set;
        }
    }
}
