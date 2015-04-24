using System;

namespace HA.COSMOS.Entities
{
    [Serializable]
    public class Address
    {
        public virtual string Type { get; set; }
        public virtual string House { get; set; }
        public virtual string Building { get; set; }
        public virtual string Street { get; set; }
        public virtual string PostOffice { get; set; }
        public virtual string State { get; set; }
        public virtual string Country { get; set; }
        public virtual string ZipCode { get; set; }
    }
}
