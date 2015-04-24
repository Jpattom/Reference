

using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using HA.Common;
using HA.COSMOS.Contracts;
using HA.COSMOS.DAContracts;
using HA.COSMOS.Entities;
using HA.COSMOS.MessageHandlers;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace HA.COSMOS.Worker.MessageHandlers.Tests
{
     [TestFixture]
    public class EditAppUsersHandlerTests
    {
         public EditAppUsersHandlerTests()
         {
             ServiceBuilder serviceBuilder = ServiceBuilder.GetInstance();
             serviceBuilder.Initialize(typeof(HA.COSMOS.Services.UserServices), typeof(HA.COSMOS.Mongo.DAL.UserDataAccessLayer));
         }

         [Test]
         public void When_Adding_App_User()
         {
             Test.Initialize();
             var mockService = MockRepository.GenerateMock<IUserServices>();
             
             var vo = new EditAppUserVO();
             vo.UserName = "Unni";
             vo.Active = true;
             vo.Email = "unni@mydomain.com";
             vo.Operation = BasicOperation.Create;
             
             
             var userContext = new COSMOSUSerContext() { UserName = "Unni", SecurityToken = "pass" };
             
             mockService.Expect(service => service.EditAppUser(vo, userContext)).Return(new UserCreationNotificationVO { UserName = "MockedUnni", Email = vo.Email, Password = "P@sssw0rd", Operation = BasicOperation.NotificationSuccess });     
             
             var message = new EditAppUsers("testSecurity", new ProcessContext(Guid.NewGuid(),
                 ProcessTypes.EditAppUser, 1, 1, true), userContext , vo);
             Test.Handler<EditAppUsersHandler>()
                 .WithExternalDependencies(h => h.UserServices = mockService)
                 .ExpectReply<ReplyMessage>(replyMessage => replyMessage.SecurityToken.Length > 0 &&
                     replyMessage.ProcessContext.ProcessId == message.ProcessContext.ProcessId)
                 .OnMessage<EditAppUsers>(m => m.AssignFrom(message));
         }

         [Test]
         public void When_Updating_App_User()
         {
             var exceptioList = AllAssemblies.Except("MQOA.DLL");

             IEnumerator<Assembly> enums = exceptioList.GetEnumerator();
             List<Assembly> asmbliesToadd = new List<Assembly>();
             asmbliesToadd.Add(enums.Current);
             while (enums.MoveNext())
             {
                 asmbliesToadd.Add(enums.Current);
             }
             Test.Initialize(asmbliesToadd.ToArray());

             var vo = new UserPasswordResetVO();
             vo.UserName = "Thommu";
             vo.OldPassword = "passw0rd";
             vo.Password = "passw0rd";
             vo.Operation = BasicOperation.Update;


             var message = new EditAppUsers("testSecurity", new ProcessContext(Guid.NewGuid(),
                 ProcessTypes.EditAppUser, 1, 1, true), new COSMOSUSerContext() { UserName = "Unni", SecurityToken = "pass" }, vo);
             Test.Handler<EditAppUsersHandler>()
                 .ExpectReply<ReplyMessage>(replyMessage => replyMessage.SecurityToken.Length > 0 &&
                     replyMessage.ProcessContext.ProcessId == message.ProcessContext.ProcessId)
                 .OnMessage<EditAppUsers>(m => m.AssignFrom(message));
         }
    }
}
