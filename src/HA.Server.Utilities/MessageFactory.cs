using HA.Common;
using HA.UtilityContracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace HA.Server.Utilities
{
    public class MessageFactory : IMessageFactory
    {
        public IDictionary MessageTypes { get; set; }

        public IBaseMessage[] GetMessages(string messageTypeName, params object[] serviceParams)
        {
            try
            {
                List<IBaseMessage> result = new List<IBaseMessage>();
                char[] separator = new char[] { ':' };
                char[] spliter = new char[] { ',' };
                string[] messageAssemblies = null;


                messageAssemblies = MessageTypes[messageTypeName].ToString().Split(separator);


                foreach (string messageAssemblyName in messageAssemblies)
                {
                    string[] typeAndAssembly = messageAssemblyName.Split(spliter);
                    Assembly messageAssembly = Assembly.Load(typeAndAssembly[1].Trim());
                    Type type = messageAssembly.GetType(typeAndAssembly[0].Trim());
                    if (serviceParams.Length > 0)
                    {
                        result.Add((IBaseMessage)Activator.CreateInstance(type, serviceParams));
                    }
                    else
                    {
                        result.Add((IBaseMessage)Activator.CreateInstance(type));
                    }
                }
                return result.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ConfigureFactory(IDictionary messageTypes)
        {
            this.MessageTypes = messageTypes;
        }
    }
}
