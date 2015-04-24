using NServiceBus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;


namespace HA.NServiceBus.Utilities
{
    public class SpecifyMessageHandlerOrdering : ISpecifyMessageHandlerOrdering
    {
        public void SpecifyOrder(Order order)
        {
            const string KEY_PREFIX = "Order";
            const string MESSAGE_HANDLER_ORDER = "MessageHandlerOrder";
            Hashtable conf = ConfigurationManager.GetSection(MESSAGE_HANDLER_ORDER) as Hashtable;
            if (conf != null)
            {
                string asmName = string.Empty;
                List<Type> typeList = new List<Type>();
                for (int i = 0; i < conf.Count; i++)
                {
                    asmName = conf[KEY_PREFIX + (i + 1).ToString()].ToString();
                    var typeAndAssmebly = asmName.Split(new char[] { ',' });
                    Assembly asm = Assembly.Load(typeAndAssmebly[0].Trim());
                    typeList.Add(asm.GetType(typeAndAssmebly[1].Trim()));
                    
                }
                order.Specify(typeList.ToArray());
            }
            else
            {
                //log
            }
        }
    }
}
