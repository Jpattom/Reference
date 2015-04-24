
using NServiceBus;
using HA.COSMOS.Messages;
namespace HA.COSMOS.MessageHandlers
{
    public class GetUserReportHandler : IHandleMessages<GetUserReport>
    {
        public void Handle(GetUserReport message)
        {
           
        }
    }
}
