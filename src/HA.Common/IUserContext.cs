using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.Common
{
    public interface IUserContext
    {
        string SecurityToken { get; set; }
        string UserName { get; set; }
        DateTime? CurrentLoginDateTimeUTC { get; set; }
        DateTime? LastLoginDateTimeUTC { get; set; }
    }
}
