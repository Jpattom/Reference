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
        private IBus bus { get; set; }

        public LoginHandler()
        {
            
        }

        public LoginHandler(IBus bus)
        {
            this.bus = bus;
        }

        public IUserServices UserServices { get; set; }

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

                    this.bus.Return(ReplyCodes.Sucess);
                    //this.Bus().Publish(reply);
                    this.bus.Reply(reply);
                }
                catch (UserServiceException ex)
                {
                    this.bus.Return(ex.ErrorNumber);
                }
                catch (Exception)
                {
                    this.bus.Return(ReplyCodes.Error);
                }
            }
            else
            {
                this.bus.Return(ReplyCodes.Error);
            }
        }


    }
}
