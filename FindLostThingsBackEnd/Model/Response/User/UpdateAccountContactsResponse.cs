using System.Collections.Generic;

namespace FindLostThingsBackEnd.Model.Response.User
{
    public class UpdateAccountContactsResponse : CommonResponse
    {
        public List<string> Updated { get; set; }
    }
}
