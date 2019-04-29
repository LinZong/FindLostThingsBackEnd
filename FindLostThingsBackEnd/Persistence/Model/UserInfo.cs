using FindLostThingsBackEnd.Model.Request.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FindLostThingsBackEnd.Persistence.Model
{
    public partial class UserInfo : AccountContacts
    {
        public long Id { get; set; }
        [JsonIgnore]
        public string Openid { get; set; }
        [JsonIgnore]
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string AndroidDevId { get; set; }
        public string Nickname { get; set; }
        //public string QQ { get; set; }
        //public string WxID { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        public int RealPersonValid { get; set; }
        public string RealPersonIdentity { get; set; }
        public int Login { get; set; }

    }
}
