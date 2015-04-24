using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using HA.COSMOS.DAContracts;
using HA.COSMOS.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HA.COSMOS.Mongo.DAL
{
    public class UserDataAccessLayer : IUserDataAccessLayer
    {
        private MongoCollection<User> rmacUsers;
        private ILog logger = LogManager.GetLogger(typeof(UserDataAccessLayer));
        public UserDataAccessLayer()
        {
            try
            {
                var dbManager = MongoDBManager.GetInstane();
                var mongoDataBase = dbManager.COSMOSDataBase;
                rmacUsers = mongoDataBase.GetCollection<User>("RMACUsers");
                rmacUsers.EnsureIndex("UserName", "Email");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

        }

        public bool AddUser(string userName, string password, string email, DateTime passwordExpiryDate)
        {
            logger.Info(string.Format("Started adding User:{0}", DateTime.UtcNow.ToString()));
            var result = true;
            try
            {
                var rmacuser = new User { UserName = userName, Password = password, Email = email, PasswordExpiryDateUTC = passwordExpiryDate, LoginTimeUTC = null };
                var insertResult = rmacUsers.Insert<User>(rmacuser);
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error(this, ex);
            }
            logger.Info(string.Format("End adding User:{0}", DateTime.UtcNow.ToString()));
            return result;

        }

        public bool AddUser(User user)
        {
            var result = false;
            try
            {
                rmacUsers.Insert(user);
                result = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                throw;
            }
            return result;

        }

        public User GetUser(string userName)
        {
            try
            {
                var query = Query<User>.EQ(user => user.UserName, userName);
                var rmacUserAlls = rmacUsers.FindAs<User>(query);
                foreach (User rmacUser in rmacUserAlls)
                {
                    if (rmacUser.UserName.Equals(userName))
                        return rmacUser;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return null;
            }
        }

        public User[] GetAllUsers()
        {
            try
            {
                List<User> results = new List<User>();
                foreach (User user in rmacUsers.FindAllAs<User>())
                {
                    results.Add(user);
                }
                return results.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return null;
            }
        }

        public bool UpdateUser(string userName, UpdateExpression<User> userUpdateExpressions)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (null == userUpdateExpressions)
                throw new ArgumentNullException("userUpdateExpressions");

            var result = true;
            try
            {
                var selectQuery = Query<User>.EQ(u => u.UserName, userName);
                var updates = new List<IMongoUpdate>();
                foreach (var key in userUpdateExpressions.Keys)
                {
                    var value = userUpdateExpressions[key];
                    var update = Update<User>.Set(key, value);
                    updates.Add(update);
                }
                var updateResult = rmacUsers.Update(selectQuery, Update.Combine(updates.ToArray()));
                result = updateResult.Ok;
            }
            catch (Exception ex)
            {
                result = false;
                System.Diagnostics.Debug.Write(ex);
            }
            return result;

        }

        public bool UpdateUser(User user)
        {
            if (null == user)
                throw new ArgumentException("user");
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrWhiteSpace(user.UserName))
                throw new ArgumentException("UserName Cannot empty to update or insert user");
            try
            {
                var result = rmacUsers.Save<User>(user);
                return result.Ok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User[] GetUsers(Expression<Func<User, bool>> comaparisonExpression)
        {
            List<User> results = new List<User>();
            var query = Query<User>.Where(comaparisonExpression);
            foreach (User user in rmacUsers.FindAs<User>(query))
            {
                results.Add(user);
            }
            return results.ToArray();
        }

        private static string GetName(Expression exp)
        {
            var body = exp as MemberExpression;
            return body.Member.Name;
        }
    }
}
