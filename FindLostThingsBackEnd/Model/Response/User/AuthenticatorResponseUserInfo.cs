using FindLostThingsBackEnd.Model.Request.User;
using Newtonsoft.Json;

namespace FindLostThingsBackEnd.Model.Response.User
{
    public class AuthenticatorResponseUserInfo : AccountContacts
    {
        public string Id { get; set; }
        [JsonIgnore]
        public string Openid { get; set; }
        [JsonIgnore]
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string AndroidDevId { get; set; }
        public string Nickname { get; set; }
        public int RealPersonValid { get; set; }
        public string RealPersonIdentity { get; set; }
    }
}
