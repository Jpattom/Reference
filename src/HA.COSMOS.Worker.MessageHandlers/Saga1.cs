using HA.Common;
using HA.COSMOS.Messages;
using HA.COSMOS.ValueObjects;
using log4net;
using NServiceBus;
using NServiceBus.Saga;
using System;
using System.Collections;
using System.Collections.Generic;


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
            Data.SagaResults = new SortedDictionary<DateTime, string>();

            Bus.SendLocal(doJobs);
            RequestTimeout<EndSaga1>(DateTime.Now.AddSeconds(1), new EndSaga1(message));
            Bus.Return(ReplyCodes.Sucess);
        }



        public void Timeout(EndSaga1 state)
        {
            Console.WriteLine("Checking Satus of {0}", state.ProcessId);
            if ((Data.SagaResults != null) && (Data.SagaResults.Values != null) && (Data.SagaResults.Values.Count != 0))
            {
                
                if (Data.SagaResults.Count == Data.NoOfProcessToBeCompletd)
                {
                    ArrayList sagaResultList = new ArrayList();
                    foreach (object obj in Data.SagaResults.Values)
                    {
                        sagaResultList.Add(obj);
                    }
                    ReplyToOriginator(new ReplyMessage(sagaResultList.ToArray(), state.SecurityToken, state.UserContext, state.ProcessContext));
                    MarkAsComplete();
                }
                else
                {
                    RequestTimeout<EndSaga1>(DateTime.Now.AddSeconds(1), state);
                }
            }
            else
            {
                RequestTimeout<EndSaga1>(DateTime.Now.AddSeconds(1), state);
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            mapper.ConfigureMapping<EndSaga1>(m => m.ProcessId).ToSaga(data => data.ProcessId);
        }
    }
}
