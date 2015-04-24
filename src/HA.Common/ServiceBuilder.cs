using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.Common
{
    public class ServiceBuilder
    {
        private static ServiceBuilder instance;
        private bool initialized;
        private Dictionary<Type, Type> creatableTypes;
        private ServiceBuilder()
        {
            creatableTypes = new Dictionary<Type, Type>();
            initialized = false;
        }

        public static ServiceBuilder GetInstance()
        {
            if (instance == null)
                instance = new ServiceBuilder();
            return instance;
        }

        public void Initialize()
        {
            //if (initialized)
            //    throw new InvalidOperationException("Already Initialized");
            ////here search for al the assembly in working directory for implentation of IBaseService and Load the assembly.
            //initialized = true;
        }

        public void Initialize(params Type[] typesTobuild)
        {
            if (initialized)
            {
                //   throw new InvalidOperationException("Already Initialized");
            }
            else
            {
                foreach (Type type in typesTobuild)
                {
                    if (type.IsClass && !type.IsAbstract)
                    {
                        var interfaces = type.GetInterfaces();
                        foreach (Type interfaceType in interfaces)
                        {
                            if (!interfaceType.Name.Equals("IBaseService"))
                            {
                                creatableTypes.Add(interfaceType, type);
                            }
                        }
                    }
                }
                initialized = true;
            }
        }

        public T Build<T>()
        {
            if (!initialized)
                throw new InvalidOperationException("Not Intilized");
            try
            {
                Type type = creatableTypes[typeof(T)];
                return (T)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to Create instace for {0}", typeof(T).Name), ex);
            }
        }

        public T Build<T>(params object[] args)
        {
            if (!initialized)
                throw new InvalidOperationException("Not Intilized");
            try
            {
                Type type = creatableTypes[typeof(T)];
                return (T)Activator.CreateInstance(type, args);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to Create instace for {0}", typeof(T).Name), ex);
            }
        }
    }
}
