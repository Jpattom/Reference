using System;

namespace HA.COSMOS.Entities
{
    [Serializable]
    public class Person
    {
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Address[] Addresses { get; set; }
    }
}
