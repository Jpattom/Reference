using HA.Common;
using HA.COSMOS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.COSMOS.Services
{
    public class AuthrizationService : IAuthorizationService
    {
        public bool AuthorizeUser(IUserContext userContext, string businessOperation)
        {
            
            if (userContext == null || string.IsNullOrEmpty(userContext.UserName) || string.IsNullOrEmpty(userContext.SecurityToken) || string.IsNullOrEmpty(businessOperation))
                throw new UnableToAuthorizeUser();
            return true;
        }
    }
}
