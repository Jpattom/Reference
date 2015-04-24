using HA.Common;
using Newtonsoft.Json;
using System;

namespace HA.COSMOS.ValueObjects
{
    [Serializable]
    [JsonObject]
    public class BaseEditUserVO : IBaseVO
    {
        public BasicOperation Operation { get; set; }
        public string UserName { get; set; }
    }

    [Serializable]
    [JsonObject]
    public class UserPasswordResetVO : BaseEditUserVO, IBaseVO
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    [JsonObject]
    public class EnableDisableUserVO : BaseEditUserVO, IBaseVO
    {
        public bool Active { get; set; }
    }

    [Serializable]
    [JsonObject]
    public class DeleteUserVO : BaseEditUserVO, IBaseVO { }

    [Serializable]
    public class EditAppUserVO : BaseEditUserVO, IBaseVO
    {
        public string Email { get; set; }
        public bool Active { get; set; }              
    }

    [JsonObject]
    [Serializable]
    public class UserCreationNotificationVO : BaseEditUserVO, IBaseVO
    {
        public string Email { get; set; }
        public string Password { get; set; }      
    }
}
