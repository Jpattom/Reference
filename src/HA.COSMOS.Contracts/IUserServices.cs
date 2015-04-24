using HA.Common;
using HA.COSMOS.ValueObjects;
using System;


namespace HA.COSMOS.Contracts
{
    public class UserServiceException : Exception
    {
        public UserServiceException()
        {
            errorNumber = 0;
        }

        protected int errorNumber = UserServiceErrorNumbers.UserServiceError;
        public int ErrorNumber
        {
            get
            {
                return errorNumber;
            }
        }
    }

    public class UserNameOrPasswordIncorect : UserServiceException 
    {
        public UserNameOrPasswordIncorect()
        {
            errorNumber = UserServiceErrorNumbers.UserNameOrPasswordIncorect;
        }
    }

    public class UserNameOrPasswordCannotBeEmpty : UserServiceException
    {
        public UserNameOrPasswordCannotBeEmpty()
        {
            errorNumber = UserServiceErrorNumbers.UserNameOrPasswordCannotBeEmpty;
        }
    }

    public class UserNameOrEmailCannotBeEmpty : UserServiceException
    {
        public UserNameOrEmailCannotBeEmpty()
        {
            errorNumber = UserServiceErrorNumbers.UserNameOrPasswordCannotBeEmpty;
        }
    }

    public class UnableToAuthorizeUser : UserServiceException
    {
        public UnableToAuthorizeUser()
        {
            errorNumber = UserServiceErrorNumbers.UnableToAutherizeUser;
        }
        public string ActionName { get; private set; }
        public string UserName { get; private set; } 
        public UnableToAuthorizeUser(string userName, string actionName)
        {
            ActionName = actionName;
            UserName = userName;
            errorNumber = UserServiceErrorNumbers.UnableToAutherizeUser;
        }
    }
   
    public class UserPasswordExpired : UserServiceException 
    {
        public UserPasswordExpired()
        {
            errorNumber = UserServiceErrorNumbers.UserPasswordExpired;
        }
    }

    public interface IUserServices: IBaseService
    {
        COSMOSUSerContext Login(LoginVO loginVo);
        BaseEditUserVO EditAppUser(BaseEditUserVO editUser, IUserContext userContext);
        EditAppUserVO[] GetAllAppUsers(IUserContext userContext);
    }
   
}
