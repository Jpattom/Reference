using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HA.Contracts
{
    internal static class KnownTypeProvider
    {
        internal static Type[] GetKnownTypes(ICustomAttributeProvider knownTypeAttributeTarget)
        {
            try
            {
                const string KEY_PREFIX = "lib";
                const string CONFIG_KNOWN_TYPES_LIBS = "ServiceKnownTypesLibs";
                Hashtable conf = System.Configuration.ConfigurationManager.GetSection(CONFIG_KNOWN_TYPES_LIBS) as Hashtable;
                if (conf != null)
                {
                    string asmName = string.Empty;
                    List<Type> lstKnownTypes = new List<Type>();
                    for (int i = 0; i < conf.Count; i++)
                    {
                        asmName = conf[KEY_PREFIX + (i + 1).ToString()].ToString();
                        Assembly asm = Assembly.Load(asmName);
                        List<Type> lstAllTypes = new List<Type>();
                        lstAllTypes.AddRange(asm.GetTypes());
                        foreach (Type t in lstAllTypes)
                        {
                            if (t.IsSerializable)
                            {
                                lstKnownTypes.Add(t);
                            }
                        }
                    }
                    return lstKnownTypes.ToArray();
                }
                else
                {
                    return new Type[] { };
                }
            }
            catch 
            {
#warning Add Logging stop throwing 
                throw;
            }
        }
    }
}
