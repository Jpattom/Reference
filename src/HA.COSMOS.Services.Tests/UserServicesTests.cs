using NUnit.Framework;
using Rhino.Mocks;
using HA.COSMOS.Contracts;
using HA.COSMOS.DAContracts;
using HA.COSMOS.Entities;
using HA.COSMOS.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.COSMOS.Services.Tests
{
    [TestFixture]
    public class UserServicesTests
    {
        #region Login Functionality Tests
        [Test]
        public void Login_When_LoginVo_IsNull()
        {
            UserServices userService = new UserServices();
            Assert.Throws(typeof(ArgumentNullException), delegate
            {
                userService.Login(null);
            });

        }

        [Test]
        public void Login_When_UserName_IsEmpty()
        {
            UserServices userService = new UserServices();
            var loginVo = new LoginVO();
            loginVo.UserName = string.Empty;
            loginVo.Password = "NotEmpty";
            Assert.Throws(typeof(UserNameOrPasswordCannotBeEmpty), delegate
            {
                userService.Login(loginVo);
            });
        }

        [Test]
        public void Login_When_Password_IsEmpty()
        {
            UserServices userService = new UserServices();
            var loginVo = new LoginVO();
            loginVo.UserName = "Ammu";
            loginVo.Password = string.Empty;
            Assert.Throws(typeof(UserNameOrPasswordCannotBeEmpty), delegate
            {
                userService.Login(loginVo);
            });

        }

        [Test]
        public void Login_When_Password_IsWrong()
        {

            var loginVo = new LoginVO();
            loginVo.UserName = "Ammu";
            loginVo.Password = "wrongpassword";
            IUserDataAccessLayer uDalMock = MockRepository.GenerateMock<IUserDataAccessLayer>();
            uDalMock.Expect(udal => udal.GetUser(loginVo.UserName)).Return(
                new User
                {
                    UserName = loginVo.UserName,
                    Password = "somethingdiffrent",
                    Active = true,
                    Deleted = false,
                    Email = "not relavent",
                    PasswordExpiryDateUTC = DateTime.UtcNow.AddDays(1)
                });
            UserServices userService = new UserServices(uDalMock);
            Assert.Throws(typeof(UserNameOrPasswordIncorect), delegate
            {
                userService.Login(loginVo);
            });
        }

        [Test]
        public void Login_When_User_Does_Not_Exixts()
        {
            var loginVo = new LoginVO();
            loginVo.UserName = "blabla";
            loginVo.Password = "wrongpassword";
            IUserDataAccessLayer uDalMock = MockRepository.GenerateMock<IUserDataAccessLayer>();
            uDalMock.Expect(udal => udal.GetUser(loginVo.UserName)).Return(null);
            UserServices userService = new UserServices(uDalMock);
            Assert.Throws(typeof(UserNameOrPasswordIncorect), delegate
            {
                userService.Login(loginVo);
            });
        }

        [Test]
        public void Login_When_User_Password_Expired()
        {
            var loginVo = new LoginVO();
            loginVo.UserName = "Ammu";
            loginVo.Password = "samepassword";
            IUserDataAccessLayer uDalMock = MockRepository.GenerateMock<IUserDataAccessLayer>();
            uDalMock.Expect(udal => udal.GetUser(loginVo.UserName)).Return(
                new User
                {
                    UserName = loginVo.UserName,
                    Password = "samepassword",
                    Active = true,
                    Deleted = false,
                    Email = "not relavent",
                    PasswordExpiryDateUTC = DateTime.UtcNow.AddDays(-1)
                });
            UserServices userService = new UserServices(uDalMock);
            Assert.Throws(typeof(UserPasswordExpired), delegate
            {
                userService.Login(loginVo);
            });
        }

        [Test]
        public void Login_When_All_Right()
        {
            var loginVo = new LoginVO();
            loginVo.UserName = "Ammu";
            loginVo.Password = "samepassword";
            IUserDataAccessLayer uDalMock = MockRepository.GenerateMock<IUserDataAccessLayer>();
            uDalMock.Expect(udal => udal.GetUser(loginVo.UserName)).Return(
                new User
                {
                    UserName = loginVo.UserName,
                    Password = "samepassword",
                    Active = true,
                    Deleted = false,
                    Email = "not relavent",
                    PasswordExpiryDateUTC = DateTime.UtcNow.AddDays(1)
                });
            UserServices userService = new UserServices(uDalMock);
            var userContext = userService.Login(loginVo);
            Assert.True(userContext.UserName == loginVo.UserName && !string.IsNullOrEmpty(userContext.SecurityToken));

        }

        #endregion Login Functionality Tests

        #region Edit AppUser Functionality Tests
        [Test]
        public void EditAppUser_When_BaseEditUserVO_Is_Null()
        {
            UserServices userService = new UserServices();
            Assert.Throws(typeof(ArgumentNullException), delegate
            {
                userService.EditAppUser(null, new COSMOSUSerContext { UserName = "notRelaventFortestCase", SecurityToken = Guid.NewGuid().ToString() });
            });
        }

        [Test]
        public void EditAppUser_When_UserContext_Is_Null()
        {
            UserServices userService = new UserServices();
            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(null,  UserServiceBusinessOperations.UpdateAppUser)).Throw(new UnableToAuthorizeUser());
            Assert.Throws(typeof(UnableToAuthorizeUser), delegate
            {
                userService.EditAppUser(new UserPasswordResetVO { UserName = "notRelaventFortestCase", Operation = BasicOperation.Update }, null);
            });
        }

        [Test]
        public void EditAppUser_When_UserContext_SecurityToken_Is_Empty()
        {
            UserServices userService = new UserServices();
            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "notRelaventFortestCase", SecurityToken = string.Empty };

            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, UserServiceBusinessOperations.UpdateAppUser)).Throw(new UnableToAuthorizeUser());
            userService.AuthorizationService = authServiceMock;
            Assert.Throws(typeof(UnableToAuthorizeUser), delegate
            {
                userService.EditAppUser(new UserPasswordResetVO { UserName = "notRelaventFortestCase", Operation = BasicOperation.Update }, userContext);
            });
        }
        
        [Test]
        public void EditAppUser_When_UnAuthorisedPerson_tryToUpdate()
        {
            
            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "UnAuthorizedPerson", SecurityToken = Guid.NewGuid().ToString() };

            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, "UPDATE_APPLICATION_USER")).Throw(new UnableToAuthorizeUser());
            UserServices userService = new UserServices();
            userService.AuthorizationService = authServiceMock;
            Assert.Throws(typeof(UnableToAuthorizeUser), delegate
            {
                try
                {
                    userService.EditAppUser(new UserPasswordResetVO { UserName = "notRelaventFortestCase", Operation = BasicOperation.Update }, userContext);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.GetType().ToString());
                    throw;
                }
            });
        }

        [Test]
        public void EditAppUser_When_UnAuthorisedPerson_tryToCreate()
        {

            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "UnAuthorizedPerson", SecurityToken = Guid.NewGuid().ToString() };

            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, "CREATE_APPLICATION_USER")).Throw(new UnableToAuthorizeUser());
            UserServices userService = new UserServices();
            userService.AuthorizationService = authServiceMock;
            Assert.Throws(typeof(UnableToAuthorizeUser), delegate
            {
                try
                {
                    userService.EditAppUser(new EditAppUserVO { UserName = "notRelaventFortestCase", Operation = BasicOperation.Create }, userContext);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.GetType().ToString());
                    throw;
                }
            });
        }

        [Test]
        public void EditAppUser_When_AuthorisedPerson_Create()
        {

            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "AuthorizedPerson", SecurityToken = Guid.NewGuid().ToString() };
            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, "CREATE_APPLICATION_USER")).Return(true);
            
            
            IUserDataAccessLayer mockDal = MockRepository.GenerateMock<IUserDataAccessLayer>();
            var editUserVo = new EditAppUserVO { UserName = "Unni", Email = "unni@myDomain.com", Active = true, Operation = BasicOperation.Create };
            mockDal.Expect(dal => dal.AddUser(null))
                .IgnoreArguments()
                .Return(true);

            UserServices userService = new UserServices(mockDal);
            userService.AuthorizationService = authServiceMock;

            var result = userService.EditAppUser(editUserVo, userContext) as UserCreationNotificationVO;

            Assert.IsTrue(
                result != null && 
                result.UserName == editUserVo.UserName && 
                result.Operation == BasicOperation.NotificationSuccess && 
                !string.IsNullOrEmpty(result.Password));
        }


        [Test]
        public void EditAppUser_When_AuthorisedPerson_Creation_Failed()
        {

            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "AuthorizedPerson", SecurityToken = Guid.NewGuid().ToString() };
            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, "CREATE_APPLICATION_USER")).Return(true);


            IUserDataAccessLayer mockDal = MockRepository.GenerateMock<IUserDataAccessLayer>();
            var editUserVo = new EditAppUserVO { UserName = "Unni", Email = "unni@myDomain.com", Active = true, Operation = BasicOperation.Create };
            mockDal.Expect(dal => dal.AddUser(null))
                .IgnoreArguments()
                .Return(false);

            UserServices userService = new UserServices(mockDal);
            userService.AuthorizationService = authServiceMock;

            var result = userService.EditAppUser(editUserVo, userContext) as EditAppUserVO;

            Assert.IsTrue(
                result != null &&
                result.UserName == editUserVo.UserName &&
                result.Operation == BasicOperation.NotificationFailure);
        }

        [Test]
        public void EditAppUser_When_AuthorisedPerson_Update()
        {

            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "AuthorizedPerson", SecurityToken = Guid.NewGuid().ToString() };
            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, "UPDATE_APPLICATION_USER")).Return(true);


            IUserDataAccessLayer mockDal = MockRepository.GenerateMock<IUserDataAccessLayer>();
            var editUserVo = new UserPasswordResetVO { UserName = "Unni", OldPassword = "P@ssw0rd", Password="newpassword", Operation = BasicOperation.Update};
            mockDal.Expect(dal => dal.UpdateUser(editUserVo.UserName, null))
                .IgnoreArguments()
                .Return(true);

            UserServices userService = new UserServices(mockDal);
            userService.AuthorizationService = authServiceMock;

            var result = userService.EditAppUser(editUserVo, userContext) as UserPasswordResetVO;

            Assert.IsTrue(
                result != null &&
                result.UserName == editUserVo.UserName &&
                result.Operation == BasicOperation.NotificationSuccess &&
                !string.IsNullOrEmpty(result.Password));
        }

        [Test]
        public void EditAppUser_When_AuthorisedPerson_Update_Failed()
        {

            COSMOSUSerContext userContext = new COSMOSUSerContext { UserName = "AuthorizedPerson", SecurityToken = Guid.NewGuid().ToString() };
            IAuthorizationService authServiceMock = MockRepository.GenerateMock<IAuthorizationService>();
            authServiceMock.Expect(authService => authService.AuthorizeUser(userContext, "UPDATE_APPLICATION_USER")).Return(true);


            IUserDataAccessLayer mockDal = MockRepository.GenerateMock<IUserDataAccessLayer>();
            var editUserVo = new UserPasswordResetVO { UserName = "Unni", OldPassword = "P@ssw0rd", Password = "newpassword", Operation = BasicOperation.Update };
            mockDal.Expect(dal => dal.UpdateUser(editUserVo.UserName, null))
                .IgnoreArguments()
                .Return(false);

            UserServices userService = new UserServices(mockDal);
            userService.AuthorizationService = authServiceMock;

            var result = userService.EditAppUser(editUserVo, userContext) as UserPasswordResetVO;

            Assert.IsTrue(
                result != null &&
                result.UserName == editUserVo.UserName &&
                result.Operation == BasicOperation.NotificationFailure);
        }
        #endregion
    }

}
