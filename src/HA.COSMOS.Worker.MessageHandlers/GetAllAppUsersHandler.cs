
using NServiceBus;
using HA.Common;
using HA.COSMOS.Contracts;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using System;

namespace HA.COSMOS.MessageHandlers
{
    public class GetAllAppUsersHandler : IHandleMessages<GetAllAppUsers>
    {
        public IUserServices UserServices { get; set; }

        public void Handle(GetAllAppUsers message)
        {
            if (UserServices == null)
            {
                ServiceBuilder sb = ServiceBuilder.GetInstance();
                UserServices = sb.Build<IUserServices>();
            }
            try
            {
                var users = UserServices.GetAllAppUsers(message.UserContext);
                var reply = this.Bus().CreateInstance<ReplyMessage>(m =>
                {

                    ProcessContextUtil.AssignTo(m.ProcessContext, message.ProcessContext);
                    m.AssignFrom(message);
                    m.SecurityToken = Guid.NewGuid().ToString();
                    m.SetServiceParams(users);
                   
                });
                this.Bus().Reply(reply);
            }
            catch (UserServiceException ex)
            {
                this.Bus().Return(ex.ErrorNumber);
            }
        }
    }
}
