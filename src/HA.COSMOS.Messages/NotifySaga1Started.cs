using HA.Common;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.COSMOS.Messages
{
    public class NotifySaga1Started: IBaseMessage, IEvent
    {
        public NotifySaga1Started(IBaseMessage baseMessage)
        {
            this.AssignFrom(baseMessage);
        }

        public Guid ProcessId { get; set; }
        private object[] serviceParams = new object[0];
        public void AssignFrom(IBaseMessage baseMessage)
        {
            this.ProcessContext = baseMessage.ProcessContext;
            this.UserContext = baseMessage.UserContext;
            this.SecurityToken = baseMessage.SecurityToken;
            this.serviceParams = baseMessage.GetServiceParams();
        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            //throw new NotImplementedException();
        }

        public object[] GetServiceParams()
        {
            return serviceParams;
        }

        public IProcessContext ProcessContext
        {
            get;
            set;
        }

        public string SecurityToken
        {
            get;
            set;
        }

        public IUserContext UserContext
        {
            get;
            set;
        }
    }
}
