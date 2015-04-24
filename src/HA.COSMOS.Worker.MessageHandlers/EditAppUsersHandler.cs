
using NServiceBus;
using HA.Common;
using HA.COSMOS.Contracts;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;

namespace HA.COSMOS.MessageHandlers
{
    public class EditAppUsersHandler : IHandleMessages<EditAppUsers>
    {
        public IUserServices UserServices{get; set;}

        public void Handle(EditAppUsers message)
        {
            ServiceBuilder sb = ServiceBuilder.GetInstance();
            if(UserServices == null)
                UserServices = sb.Build<IUserServices>();
            try
            {
                var editUserVO = message.MessagesVo;
                var result = UserServices.EditAppUser(editUserVO, message.UserContext);
                var reply = this.Bus().CreateInstance<ReplyMessage>(m =>
                {
                    ProcessContextUtil.AssignTo(m.ProcessContext, message.ProcessContext);
                    m.AssignFrom(message);
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
