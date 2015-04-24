using HA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.COSMOS.Contracts
{
    public interface IJobServices: IBaseService
    {
        void UpdateJobStatus(string jobName, string jobstatus);
        void UpdateJobResult(string jobName, object jobResult);
    }
}
