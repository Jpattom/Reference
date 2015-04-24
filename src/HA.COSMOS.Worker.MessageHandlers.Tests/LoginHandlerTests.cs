using NUnit.Framework;
using NServiceBus.Testing;
using HA.COSMOS.Worker.MessageHandlers;
using System;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using HA.Common;
using HA.COSMOS.Contracts;

namespace HA.COSMOS.Worker.MessageHandlers.Tests
{

    [TestFixture]
    public class LoginHandlerTests
    {
        public LoginHandlerTests()
        {
            ServiceBuilder serviceBuilder = ServiceBuilder.GetInstance();
            serviceBuilder.Initialize(typeof(HA.COSMOS.Services.UserServices), typeof(HA.COSMOS.Mongo.DAL.UserDataAccessLayer));
        }
         
        [Test]
        public void When_LoginVo_IsNull()
        {
            Test.Initialize();
            Test.Handler<LoginHandler>()
                .ExpectReturn<int>(code => code == ReplyCodes.Error)
                .OnMessage<Login>(m => m = null);

        }

        [Test]
        public void When_UserName_IsEmpty()
        {
            Test.Initialize();
            var loginVo = new LoginVO();
            loginVo.UserName = string.Empty;
            loginVo.Password = "NotEmpty";
            var loginMessage = new Login(loginVo, string.Empty, null, new ProcessContext(Guid.NewGuid(), ProcessTypes.Login, 1, 1, true));
            Test.Handler<LoginHandler>()
                .ExpectReturn<int>(code => code == UserServiceErrorNumbers.UserNameOrPasswordCannotBeEmpty)
                .OnMessage<Login>(m => m.AssignFrom(loginMessage));
        }

        [Test]
        public void When_Password_IsEmpty()
        {
            Test.Initialize();
            var loginVo = new LoginVO();
            loginVo.UserName = "Ammu";
            loginVo.Password = string.Empty;
            var loginMessage = new Login(loginVo, string.Empty, null, new ProcessContext(Guid.NewGuid(), ProcessTypes.Login, 1, 1, true));
            Test.Handler<LoginHandler>()
                .ExpectReturn<int>(code => code == UserServiceErrorNumbers.UserNameOrPasswordCannotBeEmpty)
                .OnMessage<Login>(m => m.AssignFrom(loginMessage));
        }

        [Test]
        public void When_Password_IsWrong()
        {
            Test.Initialize();
            var loginVo = new LoginVO();
            loginVo.UserName = "Ammu";
            loginVo.Password = "wrongpassword";
            var loginMessage = new Login(loginVo, string.Empty, null, new ProcessContext(Guid.NewGuid(), ProcessTypes.Login, 1, 1, true));
            Test.Handler<LoginHandler>()
                .ExpectReturn<int>(code => code == UserServiceErrorNumbers.UserNameOrPasswordIncorect)
                .OnMessage<Login>(m => m.AssignFrom(loginMessage));
        }

        [Test]
        public void When_User_Does_Not_Exixts()
        {
            Test.Initialize();
            var loginVo = new LoginVO();
            loginVo.UserName = "nouser";
            loginVo.Password = "wrongpassword";
            var loginMessage = new Login(loginVo, string.Empty, null, new ProcessContext(Guid.NewGuid(), ProcessTypes.Login, 1, 1, true));
            Test.Handler<LoginHandler>()
                .ExpectReturn<int>(code => code == UserServiceErrorNumbers.UserNameOrPasswordIncorect)
                .OnMessage<Login>(m => m.AssignFrom(loginMessage));
        }

        [Test]
        public void When_User_Password_Expired()
        {
            Test.Initialize();
            var loginVo = new LoginVO();
            loginVo.UserName = "Unni";
            loginVo.Password = "password";
            var loginMessage = new Login(loginVo, string.Empty, null, new ProcessContext(Guid.NewGuid(), ProcessTypes.Login, 1, 1, true));
            Test.Handler<LoginHandler>()
                .ExpectReturn<int>(code => code == UserServiceErrorNumbers.UserPasswordExpired)
                .OnMessage<Login>(m => m.AssignFrom(loginMessage));
        }

        [Test]
        public void When_All_Right()
        {
            Test.Initialize();
            var loginVo = new LoginVO();
            loginVo.UserName = "Thommu";
            loginVo.Password = "passw0rd";
            var loginMessage = new Login(loginVo, string.Empty, null, 
                new ProcessContext(Guid.NewGuid(), ProcessTypes.Login, 1, 1, true));
            Test.Handler<LoginHandler>()
                .ExpectReply<ReplyMessage>(replyMessage =>
                {
                    var rmacuserContext = replyMessage.UserContext;
                    Assert.AreEqual(loginVo.UserName, rmacuserContext.UserName);
                    //System.Diagnostics.Debug.WriteLine(rmacuserContext.UserName);
                    return replyMessage.SecurityToken.Length > 0 &&
                        replyMessage.ProcessContext.ProcessId == loginMessage.ProcessContext.ProcessId;
                })
                .OnMessage<Login>(m => m.AssignFrom(loginMessage));
        }
    }
}
