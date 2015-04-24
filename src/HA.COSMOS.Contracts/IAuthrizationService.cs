using HA.Common;

namespace HA.COSMOS.Contracts
{
    public interface IAuthorizationService: IBaseService
    {
        bool AuthorizeUser(IUserContext userContext, string businessOperation);
    }
}
