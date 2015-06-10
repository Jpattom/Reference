using HA.COSMOS.Messages;
using NServiceBus;
using NServiceBus.Saga;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HA.COSMOS.MessageHandlers
{

    internal static class CPUIntensiveFuctions
    {
        internal static void SlowMeDown(int seconds)
        {
            var end = DateTime.Now + TimeSpan.FromSeconds(seconds);
            while (DateTime.Now < end)
                /*nothing here */
                ;
        }
    }

    public class DoWhenSaga1Start : IHandleMessages<NotifySaga1Started>
    {
        private IBus bus { get; set; }

        public DoWhenSaga1Start()
        {
        }

        public DoWhenSaga1Start(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(NotifySaga1Started message)
        {
            bus.SendLocal(new DoJob(message));
        }
    }

    public class DoJob1 : Saga<MySagaData>,
        IHandleMessages<DoJob>
    {

        public void Handle(DoJob message)
        {
            CPUIntensiveFuctions.SlowMeDown(2);
            this.Data.SagaResults[1] = string.Format("Result of {0} executed at {1} in {2}", this.GetType().Name, DateTime.Now.ToString("hh.mm.ss.ffffff"), Process.GetCurrentProcess().Id);

        }

       
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            mapper.ConfigureMapping<DoJob>(m => m.ProcessId).ToSaga(s => s.ProcessId);
        }
    }

    public class DoJob2 : Saga<MySagaData>,
        IHandleMessages<DoJob>
    {

        public void Handle(DoJob message)
        {
            CPUIntensiveFuctions.SlowMeDown(10);
            this.Data.SagaResults[2] = string.Format("Result of {0} executed at {1} in {2}", this.GetType().Name, DateTime.Now.ToString("hh.mm.ss.ffffff"), Process.GetCurrentProcess().Id);
            
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            mapper.ConfigureMapping<DoJob>(m => m.ProcessId).ToSaga(s => s.ProcessId);
        }
    }

    public class DoJob3 : Saga<MySagaData>,
        IHandleMessages<DoJob>
    {

        public void Handle(DoJob message)
        {
            CPUIntensiveFuctions.SlowMeDown(4);
            this.Data.SagaResults[3] = string.Format("Result of {0} executed at {1} in {2}", this.GetType().Name, DateTime.Now.ToString("hh.mm.ss.ffffff"), Process.GetCurrentProcess().Id);
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            mapper.ConfigureMapping<DoJob>(m => m.ProcessId).ToSaga(s => s.ProcessId);
        }
    }

    public class DoJob4 : Saga<MySagaData>,
        IHandleMessages<DoJob>
    {

        public void Handle(DoJob message)
        {
            CPUIntensiveFuctions.SlowMeDown(3);
            this.Data.SagaResults[4] = string.Format("Result of {0} executed at {1} in {2}", this.GetType().Name, DateTime.Now.ToString("hh.mm.ss.ffffff"), Process.GetCurrentProcess().Id);
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            mapper.ConfigureMapping<DoJob>(m => m.ProcessId).ToSaga(s => s.ProcessId);
        }
    }
}
