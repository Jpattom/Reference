using NServiceBus;
using HA.Common;
using HA.COSMOS.Contracts;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using System;


namespace HA.COSMOS.Worker.MessageHandlers
{
    public class LoginHandler : IHandleMessages<Login>
    {
        public IUserServices UserServices { get; set; }
        IBus Bus { get; set; }
        public void Handle(Login message)
        {
            if (message.LoginVO != null)
            {
                if (UserServices == null)
                {
                    ServiceBuilder sb = ServiceBuilder.GetInstance();
                    UserServices = sb.Build<IUserServices>();
                }
                try
                {
                    var userContext = UserServices.Login(message.LoginVO);
                    var reply = new ReplyMessage();
                    reply.AssignFrom(message);
                    ProcessContextUtil.AssignTo(reply.ProcessContext, message.ProcessContext);
                    reply.SecurityToken = userContext.SecurityToken;
                    reply.UserContext = userContext;

                    this.Bus.Return(ReplyCodes.Sucess);
                    //this.Bus().Publish(reply);
                    this.Bus.Reply(reply);
                }
                catch (UserServiceException ex)
                {
                    this.Bus.Return(ex.ErrorNumber);
                }
                catch (Exception)
                {
                    this.Bus.Return(ReplyCodes.Error);
                }
            }
            else
            {
                this.Bus.Return(ReplyCodes.Error);
            }
        }
    }
}
