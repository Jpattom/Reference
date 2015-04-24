using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.Common
{
    [JsonObject]
    public interface IProcessContext
    {
        Guid ProcessId { get; set; }
        string ProcessType { get; set; }
        uint TotalNumberOfMessages { get; set; }
        uint MessageNumber { get; set; }
        bool IsLastMessage { get; set; }
    }

}
