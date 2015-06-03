using HA.Common;
using Newtonsoft.Json;
using System;

namespace HA.COSMOS.ValueObjects
{
    public class UserServiceBusinessOperations
    {
        public const string UpdateAppUser = "UPDATE_APPLICATION_USER";
        public const string CreateAppUser = "CREATE_APPLICATION_USER";
    }

    public class UserServiceErrorNumbers
    {
        public const int UserServiceErrorNumberBase = 1000;
        public const int Default = QuickReplyCodes.None + UserServiceErrorNumberBase;
        public const int UserServiceSuccess = QuickReplyCodes.Sucess + UserServiceErrorNumberBase;
        public const int UserServiceError = QuickReplyCodes.Failed + UserServiceErrorNumberBase;
        public const int UserNameOrPasswordIncorect = UserServiceError + 1;
        public const int UserPasswordExpired = UserServiceError + 2;
        public const int UnableToAutherizeUser = UserServiceError + 3;
        public const int UserNameOrPasswordCannotBeEmpty = UserServiceError + 4;
        public const int UserNameOrEmailCannotBeEmpty = UserServiceError + 5;
    }

    public static class ProcessTypes
    {
        public const string Login = "Login";
        public const string EditAppUser = "EditAppUser";
        public const string GetAllAppUsers = "GetAppUser";
        public const string DoJob = "DoJob";
    }

    public enum CacheableEntities : int
    {
        Nothing = 0,
        User = 1
    }

   
    public enum BasicOperation
    {
        Create,
        Read,
        Update,
        Delete,
        NotificationSuccess,
        NotificationFailure

    }
}
