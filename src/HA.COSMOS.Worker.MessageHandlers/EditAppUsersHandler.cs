
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

        public IBus Bus { get; set; }

        
        public void Handle(EditAppUsers message)
        {
            ServiceBuilder sb = ServiceBuilder.GetInstance();
            if(UserServices == null)
                UserServices = sb.Build<IUserServices>();
            try
            {
                var editUserVO = message.MessagesVo;
                var result = UserServices.EditAppUser(editUserVO, message.UserContext);
                var reply = new ReplyMessage
                {
                    SecurityToken = message.SecurityToken,
                    UserContext = message.UserContext,
                    ProcessContext = message.ProcessContext

                };

                ProcessContextUtil.AssignTo(reply.ProcessContext, message.ProcessContext);
                reply.AssignFrom(message);
                this.Bus.Return(ReplyCodes.Sucess);
                this.Bus.Reply(reply);

            }
            catch (UserServiceException ex)
            {
                this.Bus.Return(ex.ErrorNumber);
            }
        }
    }
}
