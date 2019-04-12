using FindLostThingsBackEnd.Persistence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Model.Request.User
{
    public class AccountContacts
    {
        public string QQ { get; set; }
        public string WxID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        
    }
}
