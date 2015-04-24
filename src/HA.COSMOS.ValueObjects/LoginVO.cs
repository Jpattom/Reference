using HA.Common;
using Newtonsoft.Json;
using System;

namespace HA.COSMOS.ValueObjects
{
    [Serializable]
    [JsonObject]
    public class LoginVO : IBaseVO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
    }
}
