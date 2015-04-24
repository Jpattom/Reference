using HA.Common;
using Newtonsoft.Json;
using System;

namespace HA.COSMOS.ValueObjects
{
    [Serializable]
    [JsonObject]
    public class ServiceMessage
    {
        private object[] serviceParams;
        public int ErrorCode { get; set; }
        public COSMOSUSerContext UserContext { get; set; }
        public object[] ServiceParams {
            get
            {
                if (serviceParams == null)
                    serviceParams = new object[0];
                return serviceParams;
            }
            set 
            {
                if(value == null)
                    serviceParams = new object[0];
                else
                    serviceParams = value;
            }
           }
        public string SecurityToken { get; set; }
        public ProcessContext ProcessContext { get; set; }

    }
}
