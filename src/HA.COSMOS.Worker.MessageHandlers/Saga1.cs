using HA.Common;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using log4net;
using NServiceBus;
using NServiceBus.Saga;
using System;
using System.Collections;


namespace HA.COSMOS.MessageHandlers
{
    

    public class Saga1 : Saga<MySagaData>, IAmStartedByMessages<StartSaga1>,
        IHandleTimeouts<EndSaga1>
    {
        private ILog logger = LogManager.GetLogger(typeof(Saga1));

        public void Handle(StartSaga1 message)
        {
            this.Data.ProcessId = message.ProcessContext.ProcessId;
            Console.WriteLine("Saga Satarted {0}", Data.ProcessId);
            var doJobs = new DoJob(message);
            doJobs.ProcessId = message.ProcessContext.ProcessId;
            this.Data.NoOfProcessToBeCompletd = 4;
            Data.SagaResults = new Hashtable(4);

            Bus.SendLocal(doJobs);
            RequestTimeout<EndSaga1>(DateTime.Now.AddSeconds(1), new EndSaga1(message));
            Bus.Return(ReplyCodes.Sucess);
        }
        
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<EndSaga1>(m => m.ProcessId).ToSaga(data => data.ProcessId);
        }
      
        public void Timeout(EndSaga1 state)
        {
            Console.WriteLine("Checking Satus of {0}", state.ProcessId);
            ArrayList sagaResultList = new ArrayList();
           
            foreach (object obj in Data.SagaResults.Values)
            {
                if (obj == null)
                {
                    RequestTimeout<EndSaga1>(DateTime.Now.AddSeconds(1), state);
                    break;
                }
                else
                {
                    sagaResultList.Add(obj);
                }

                if (sagaResultList.Count == Data.NoOfProcessToBeCompletd)
                {
                    ReplyToOriginator(new ReplyMessage(sagaResultList.ToArray(), state.SecurityToken, state.UserContext, state.ProcessContext));
                    MarkAsComplete();
                }
            }
           // 
        }
    }
}
