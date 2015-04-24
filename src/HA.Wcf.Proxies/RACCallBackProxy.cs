using HA.Contracts;
using System;


namespace HA.WCF.Proxies
{
    public class RACCallBackProxy : IService
    {
        public void InvokeService(IWCFMessage replyMessage)
        {
           Console.WriteLine(string.Format("reply {0} of {1} ProcessId {2} ProcessType {3}", replyMessage.ProcessContext.MessageNumber, 
               replyMessage.ProcessContext.TotalNumberOfMessages, replyMessage.ProcessContext.ProcessId, replyMessage.ProcessContext.ProcessType));
                           
        }
    }
}
