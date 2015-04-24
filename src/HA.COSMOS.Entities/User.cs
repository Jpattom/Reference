using System;

namespace HA.COSMOS.Entities
{
    [Serializable]
    public class User
    {
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime? PasswordExpiryDateUTC { get; set; }
        public virtual DateTime? LoginTimeUTC { get; set; }
        public virtual string Email { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual bool Active { get; set; }
        //public virtual Person PersonalInfo { get; set; }
    }
}
