using HA.COSMOS.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HA.COSMOS.DAContracts
{
    public interface IUserDataAccessLayer
    {
        User GetUser(string userName);

        bool AddUser(string userName, string password, string emailId, DateTime passwordExpiryDate);

        bool AddUser(User user);

        bool UpdateUser(string userName, UpdateExpression<User> userUpdateExpressions);

        User[] GetAllUsers();

        User[] GetUsers(Expression<Func<User, bool>> comparisonExpressions);
    }
}
