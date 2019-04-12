using FindLostThingsBackEnd.Persistence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Model.Response.User
{
    public class UserInfoResponse : CommonResponse
    {
        public UserInfo UserInfo { get; set; }

    }
}
