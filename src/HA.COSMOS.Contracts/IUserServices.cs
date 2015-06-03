using HA.Common;
using HA.COSMOS.ValueObjects;
using System;


namespace HA.COSMOS.Contracts
{
    [Serializable]
    public class UserServiceException : Exception
    {
        public UserServiceException()
        {
            _errorNumber = UserServiceErrorNumbers.UserServiceError;
        }

        protected int _errorNumber;
        public int ErrorNumber
        {
            get
            {
                return _errorNumber;
            }
        }

    }

    public class UserNameOrPasswordIncorect : UserServiceException 
    {
        public UserNameOrPasswordIncorect()
        {
            _errorNumber = UserServiceErrorNumbers.UserNameOrPasswordIncorect;
        }
    }

    public class UserNameOrPasswordCannotBeEmpty : UserServiceException
    {
        public UserNameOrPasswordCannotBeEmpty()
        {
            _errorNumber = UserServiceErrorNumbers.UserNameOrPasswordCannotBeEmpty;
        }
    }

    public class UserNameOrEmailCannotBeEmpty : UserServiceException
    {
        public UserNameOrEmailCannotBeEmpty()
        {
            _errorNumber = UserServiceErrorNumbers.UserNameOrPasswordCannotBeEmpty;
        }
    }

    public class UnableToAuthorizeUser : UserServiceException
    {
        public UnableToAuthorizeUser()
        {
            _errorNumber = UserServiceErrorNumbers.UnableToAutherizeUser;
        }
        public string ActionName { get; private set; }
        public string UserName { get; private set; } 
        public UnableToAuthorizeUser(string userName, string actionName)
        {
            ActionName = actionName;
            UserName = userName;
            _errorNumber = UserServiceErrorNumbers.UnableToAutherizeUser;
        }
    }
   
    public class UserPasswordExpired : UserServiceException 
    {
        public UserPasswordExpired()
        {
            _errorNumber = UserServiceErrorNumbers.UserPasswordExpired;
        }
    }

    public interface IUserServices: IBaseService
    {
        COSMOSUSerContext Login(LoginVO loginVo);
        BaseEditUserVO EditAppUser(BaseEditUserVO editUser, IUserContext userContext);
        EditAppUserVO[] GetAllAppUsers(IUserContext userContext);
    }
   
}
