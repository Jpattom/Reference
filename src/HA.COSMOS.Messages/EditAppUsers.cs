
using NServiceBus;
using HA.Common;
using HA.COSMOS.ValueObjects;
using System;
namespace HA.COSMOS.Messages
{
    [Serializable]
    public class EditAppUsers: IBaseMessage, ICommand
    {
        public EditAppUsers()
        {
            ProcessContext = new ProcessContext();
        }

        public EditAppUsers(string securityToken, IProcessContext processContext, IUserContext userContext, BaseEditUserVO editUserVo)
        {
            SecurityToken = securityToken;
            ProcessContext = processContext;
            UserContext = userContext;
            MessagesVo = editUserVo;
            if (editUserVo != null) 
            {
                serviceParams = new object[]{editUserVo};
            }
        }

        public BaseEditUserVO MessagesVo { get; set; }

        private object[] serviceParams;

        public string SecurityToken
        {
            get;
            set;
        }

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
            if (baseMessage is EditAppUsers)
            {
                this.MessagesVo = ((EditAppUsers)baseMessage).MessagesVo;
            }
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            ProcessContextUtil.AssignTo(baseMessage.ProcessContext, this.ProcessContext);
            baseMessage.SecurityToken = this.SecurityToken;
            baseMessage.UserContext = this.UserContext;
            if (baseMessage is EditAppUsers)
            {
                ((EditAppUsers)baseMessage).MessagesVo = this.MessagesVo;
            }
        }

        public IUserContext UserContext
        {
            get;
            set;
        }
    }
}
