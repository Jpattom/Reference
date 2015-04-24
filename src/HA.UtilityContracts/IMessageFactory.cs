using HA.Common;
using System;
using System.Collections;

namespace HA.UtilityContracts
{
    public interface IMessageFactory
    {
        IBaseMessage[] GetMessages(string messageTypeName, params object[] serviceParams);
        void ConfigureFactory(IDictionary messageTypes); 
    }
}
