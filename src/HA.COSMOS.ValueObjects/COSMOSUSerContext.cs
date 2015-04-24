using HA.Common;
using Newtonsoft.Json;
using System;

namespace HA.COSMOS.ValueObjects
{
    [Serializable]
    [JsonObject]
    public class COSMOSUSerContext : IUserContext, IBaseVO
    {
        public string UserName { get; set; }
        public string SecurityToken { get; set; }
        public DateTime? CurrentLoginDateTimeUTC { get; set; }
        public DateTime? LastLoginDateTimeUTC { get; set; }
    }
}
