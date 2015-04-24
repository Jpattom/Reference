using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.Common
{
     [JsonObject]
    public interface IBaseMessage
    {
        string SecurityToken { get; set; }
        object[] GetServiceParams();
        IProcessContext ProcessContext { get; set; }
        void AssignFrom(IBaseMessage baseMessage);
        void AssignTo(IBaseMessage baseMessage);
        IUserContext UserContext { get; set; }
    }
}
