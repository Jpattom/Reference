using NServiceBus.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using HA.Common;
using HA.COSMOS.Contracts;
using HA.COSMOS.MessageHandlers;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using System;
using System.Collections.Generic;

namespace HA.COSMOS.Worker.MessageHandlers.Tests
{
    [TestFixture]
    public class GetAllAppUsersHandlerTests
    {
        public GetAllAppUsersHandlerTests()
        {
            ServiceBuilder serviceBuilder = ServiceBuilder.GetInstance();
            serviceBuilder.Initialize(typeof(HA.COSMOS.Services.UserServices), typeof(HA.COSMOS.Mongo.DAL.UserDataAccessLayer));
        }

        [Test]
        public void When_All_Is_Well()
        {
            Test.Initialize();
            var message = new GetAllAppUsers(new ProcessContext(Guid.NewGuid(),
                ProcessTypes.GetAllAppUsers, 1, 1, true), new COSMOSUSerContext() { UserName = "Unni", SecurityToken = "pass" });
            Test.Handler<GetAllAppUsersHandler>()
                .ExpectReply<ReplyMessage>(replyMessage =>
                {
                    foreach (EditAppUserVO user in replyMessage.GetServiceParams())
                    {
                        System.Diagnostics.Debug.WriteLine(user.UserName);
                    }

                    return
                        replyMessage.SecurityToken.Length > 0 &&
                        replyMessage.ProcessContext.ProcessId == message.ProcessContext.ProcessId &&
                        replyMessage.GetServiceParams().Length > 0;
                }
                    )
                .OnMessage<GetAllAppUsers>(m => m.AssignFrom(message));
        }


        [Test]
        public void When_There_Are_Large_number_of_Users()
        {
            Test.Initialize();
            var mockService = MockRepository.GenerateMock<IUserServices>();

            var message = new GetAllAppUsers(new ProcessContext(Guid.NewGuid(),
                ProcessTypes.GetAllAppUsers, 1, 1, true), new COSMOSUSerContext() { UserName = "Unni", SecurityToken = "pass" });

            mockService.Expect(service => service.GetAllAppUsers(message.UserContext)).Return(GetEditAppUserVOsForTest(1000));     
            
            Test.Handler<GetAllAppUsersHandler>()
                .WithExternalDependencies(h => h.UserServices = mockService)
                .ExpectReply<ReplyMessage>(replyMessage =>
                {
                    foreach (EditAppUserVO user in replyMessage.GetServiceParams())
                    {
                        System.Diagnostics.Debug.WriteLine(user.UserName);
                    }

                    return
                        replyMessage.SecurityToken.Length > 0 &&
                        replyMessage.ProcessContext.ProcessId == message.ProcessContext.ProcessId &&
                        replyMessage.GetServiceParams().Length > 0;
                }
                    )
                .OnMessage<GetAllAppUsers>(m => m.AssignFrom(message));
        }


        private EditAppUserVO[] GetEditAppUserVOsForTest(int numberofDistictVos)
        {
            List<EditAppUserVO> result = new List<EditAppUserVO>();
            
            for(int i = 0; i < numberofDistictVos; i++)
            {
                string userName = string.Format("Mock{0}", i);
                string email = string.Format("{0}@email.com", userName);

                result.Add(new EditAppUserVO { UserName = userName, Email = email, Operation = BasicOperation.Read, Active = true });
            }

            return result.ToArray();
        }

    }
}
