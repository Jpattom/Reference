
using NServiceBus;
using HA.Common;
namespace HA.COSMOS.Messages
{
    public class GetUserReport : IBaseMessage, ICommand
    {
        private object[] serviceParams;
        public GetUserReport()
        {
            ProcessContext = new ProcessContext();
            serviceParams = new object[0];
        }

        public GetUserReport(IProcessContext processContext, IUserContext userContext)
        {
            ProcessContext = processContext;
            UserContext = userContext;
            serviceParams = new object[0];
        }

        public string SecurityToken { get; set; }

        public object[] GetServiceParams()
        {
            return serviceParams;
        }

        public IProcessContext ProcessContext
        {
            get;
            set;
        }

        public void AssignFrom(IBaseMessage baseMessage)
        {
            ProcessContextUtil.AssignFrom(baseMessage.ProcessContext, this.ProcessContext);
            this.SecurityToken = baseMessage.SecurityToken;
            this.UserContext = baseMessage.UserContext;

        }

        public void AssignTo(IBaseMessage baseMessage)
        {
            ProcessContextUtil.AssignTo(baseMessage.ProcessContext, this.ProcessContext);
            baseMessage.SecurityToken = this.SecurityToken;
            baseMessage.UserContext = this.UserContext;
        }

        public IUserContext UserContext { get; set; }
    }
}
