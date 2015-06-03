using log4net;
using HA.Common;
using HA.COSMOS.Contracts;
using HA.COSMOS.DAContracts;
using HA.COSMOS.Entities;
using HA.COSMOS.ValueObjects;
using System;
using System.Collections.Generic;

namespace HA.COSMOS.Services
{
    
    public class UserServices : IUserServices
    {
        private COSMOSUSerContext userContext;
        private ILog logger = LogManager.GetLogger(typeof(UserServices));
        public UserServices()
        {
        }

        public UserServices(IUserContext userContext)
        {
            if (userContext is COSMOSUSerContext)
            {
                this.userContext = userContext as COSMOSUSerContext;
            }
        }

        /// <summary>
        /// Use while Testing for Mocking User Data AccesLayer
        /// </summary>
        /// <param name="userDataAcessLayer">Mocked User dataccess Layer for testing other wise initilized using Dependency Injection</param>
        public UserServices(IUserDataAccessLayer userDataAcessLayer)
        {
            this.UserDataAcessLayer = userDataAcessLayer;
        }

        /// <summary>
        /// Use while Testing for Mocking User Data AccesLayer
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="userDataAcessLayer"></param>
        public UserServices(IUserContext userContext, IUserDataAccessLayer userDataAcessLayer)
        {
            if (userContext is COSMOSUSerContext)
            {
                this.userContext = userContext as COSMOSUSerContext;
            }
            this.UserDataAcessLayer = userDataAcessLayer;
        }

        private IUserDataAccessLayer userDataAcessLayer;
        private IUserDataAccessLayer UserDataAcessLayer
        {
            get { return userDataAcessLayer ?? (userDataAcessLayer = ServiceBuilder.GetInstance().Build<IUserDataAccessLayer>()); }
            set { userDataAcessLayer = value; }
        }

        public COSMOSUSerContext Login(LoginVO loginVo)
        {
            if (null == loginVo)
                throw new ArgumentNullException("LoginVO");

            logger.Info(string.Format("Login business Started {0}:", DateTime.UtcNow.ToString()));


            try
            {
                if (!string.IsNullOrEmpty(loginVo.Password) && !string.IsNullOrEmpty(loginVo.UserName))
                {


                    var user = UserDataAcessLayer.GetUser(loginVo.UserName);
                    if (user != null && loginVo.Password.Equals(user.Password))
                    {
                        if (user.PasswordExpiryDateUTC < DateTime.UtcNow)
                        {
                            throw new UserPasswordExpired();
                        }
                        var userContext = new COSMOSUSerContext();
                        userContext.UserName = user.UserName;
                        userContext.SecurityToken = Guid.NewGuid().ToString();
                        UserDataAcessLayer.UpdateUser(user.UserName, new UpdateExpression<User>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
                        { 
                            { u => u.LoginTimeUTC, DateTime.UtcNow } 
                        });
                        return userContext;
                    }
                    else
                    {
                        throw new UserNameOrPasswordIncorect();
                    }
                }
                else
                {
                    throw new UserNameOrPasswordCannotBeEmpty();
                }
            }
            catch (Exception ex)
            {
                logger.Error(this, ex);
                throw; 
            }
            finally
            {
                logger.Info(string.Format("Login business end {0}:", DateTime.UtcNow.ToString()));
            }


        }

        public IAuthorizationService AuthorizationService { get; set; }

        
        public BaseEditUserVO EditAppUser(BaseEditUserVO editUser, IUserContext userContext)
        {
            if (null == editUser)
                throw new ArgumentNullException("editUser");

            if (AuthorizationService == null)
                AuthorizationService = new AuthrizationService();

            var result = editUser;

            if (editUser is UserPasswordResetVO && editUser.Operation == BasicOperation.Update && 
                AuthorizationService.AuthorizeUser(userContext, UserServiceBusinessOperations.UpdateAppUser))
            {
                var passwordResetVo = editUser as UserPasswordResetVO;
                if (UserDataAcessLayer.UpdateUser(passwordResetVo.UserName, new UpdateExpression<User> { { u => u.Password, passwordResetVo.Password }, { u => u.PasswordExpiryDateUTC, DateTime.UtcNow.AddMonths(1) } }))
                {
                    result.Operation = BasicOperation.NotificationSuccess;
                }
                else
                {
                    result.Operation = BasicOperation.NotificationFailure;
                }

            }

            if (editUser is EditAppUserVO && editUser.Operation == BasicOperation.Create && 
                AuthorizationService.AuthorizeUser(userContext, UserServiceBusinessOperations.CreateAppUser))
            {
                var editUserVo = editUser as EditAppUserVO;

                if (string.IsNullOrEmpty(editUserVo.Email) || string.IsNullOrWhiteSpace(editUserVo.Email) ||
                    string.IsNullOrEmpty(editUser.UserName) || string.IsNullOrEmpty(editUser.UserName))
                    throw new UserNameOrEmailCannotBeEmpty();
#warning hard coding develop fuctionality generate password and password expiry policy injection
                var password = "p@ssword1";
                var passwordExpirydate = DateTime.UtcNow.AddMonths(1);

                var user = new User
                {
                    UserName = editUserVo.UserName,
                    Active = true,
                    Deleted = false,
                    Email = editUserVo.Email,
                    Password = password,
                    LoginTimeUTC = null,
                    PasswordExpiryDateUTC = passwordExpirydate
                };

                if (UserDataAcessLayer.AddUser(user))
                {
                    result = new UserCreationNotificationVO
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Password = user.Password,
                        Operation = BasicOperation.NotificationSuccess
                    };
                }
                else
                {
                    result.Operation = BasicOperation.NotificationFailure;
                }
            }
            return result;


        }

        public EditAppUserVO[] GetAllAppUsers(IUserContext userContext)
        {
            if (userContext != null)
            {
                var result = new List<EditAppUserVO>();

                IUserDataAccessLayer userDataAcessLayer = ServiceBuilder.GetInstance().Build<IUserDataAccessLayer>();
                var allUsers = userDataAcessLayer.GetAllUsers();
                foreach (User user in allUsers)
                {
                    result.Add(new EditAppUserVO() { UserName = user.UserName, Active = user.Active, Email = user.Email });
                }
                return result.ToArray();
            }
            else
            {
                throw new UnableToAuthorizeUser();
            }

        }

        public EditAppUserVO[] GetUsersLoggedInBetween(DateTime startDate, DateTime endDate)
        {
            if (userContext != null)
            {
                var result = new List<EditAppUserVO>();

                IUserDataAccessLayer userDataAcessLayer = ServiceBuilder.GetInstance().Build<IUserDataAccessLayer>();
                var theUsers = userDataAcessLayer.GetUsers(u => (u.LoginTimeUTC >= startDate) && (u.LoginTimeUTC <= endDate));

                foreach (User user in theUsers)
                {
                    result.Add(new EditAppUserVO() { UserName = user.UserName, Active = user.Active, Email = user.Email });
                }
                return result.ToArray();
            }
            else
            {
                throw new UnableToAuthorizeUser();
            }
        }
    }
}
