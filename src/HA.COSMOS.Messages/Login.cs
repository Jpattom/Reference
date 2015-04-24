using NServiceBus;
using HA.Common;
using HA.COSMOS.ValueObjects;
using System;

namespace HA.COSMOS.Messages
{
    [Serializable]
    public class Login : IBaseMessage, ICommand
    {

        private object[] serviceParams;
        public Login()
        {
            this.ProcessContext = new ProcessContext();
        }

        public Login(LoginVO loginVO)
        {
            this.ProcessContext = new ProcessContext();
            this.LoginVO = loginVO;
            serviceParams = new object[] { loginVO };
        }

        public Login(LoginVO loginVO, string securityToken, IUserContext userContext, IProcessContext processContext)
        {
            this.ProcessContext = processContext;
            this.LoginVO = loginVO;
            this.SecurityToken = securityToken;
            this.UserContext = userContext;
            serviceParams = new object[] { loginVO };
        }

        public Login(LoginVO loginVO, IProcessContext processContext, IUserContext userContext, string securityToken)
        {
            this.LoginVO = loginVO;
            this.ProcessContext = processContext;
            this.UserContext = userContext;
            this.SecurityToken = securityToken;
        }

        public LoginVO LoginVO { get; set; }

        public object[] GetServiceParams()
        {
            return serviceParams;
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
            if (baseMessage is Login)
            {
                this.LoginVO = ((Login)baseMessage).LoginVO;
            }
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            ProcessContextUtil.AssignTo(baseMessage.ProcessContext, this.ProcessContext);
            baseMessage.SecurityToken = this.SecurityToken;
            baseMessage.UserContext = this.UserContext;
            if (baseMessage is Login)
            {
                ((Login)baseMessage).LoginVO = this.LoginVO;
            }
        }
    }
}
