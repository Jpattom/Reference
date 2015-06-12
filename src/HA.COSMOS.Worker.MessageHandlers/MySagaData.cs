using NServiceBus.Saga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.COSMOS.MessageHandlers
{
    [Serializable]
    public class MySagaData : IContainSagaData
    {

        public MySagaData()
        {

        }

        [Unique]
        public Guid ProcessId
        {
            get;
            set;
        }

        public Guid Id { get; set; }

        public string OriginalMessageId { get; set; }

        public string Originator { get; set; }

        internal SortedDictionary<DateTime, string> SagaResults { get; set; }

        internal int NoOfProcessToBeCompletd { get; set; }
    }

}
