using FindLostThingsBackEnd.Persistence.Model;
using System.Linq;

namespace FindLostThingsBackEnd.Model.Response.User
{
    public class UnAuthenticatedUserResponse : CommonResponse
    {
        public IQueryable<UserInfo> UserList { get; set; }
    }
}
