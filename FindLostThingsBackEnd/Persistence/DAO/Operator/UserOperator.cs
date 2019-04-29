using FindLostThingsBackEnd.Persistence.DAO.Context;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Persistence.DAO.Operator
{
    public class UserOperator : IFindLostThingsDbOperator
    {
        private readonly LostContext context;
        public UserOperator(LostContext ctx)
        {
            context = ctx;
        }

        public UserInfo GetUserInfo(string OpenID)
        {
            return context.UserInfo.FirstOrDefault(x => x.Openid == OpenID);
        }

        public UserInfo GetUserInfo(long UserID)
        {
            return context.UserInfo.FirstOrDefault(x => x.Id == UserID);
        }

        public IQueryable<UserInfo> GetUnAuthenticatedUser()
        {
            return context.UserInfo.Where(x => x.RealPersonValid == 0 && !string.IsNullOrEmpty(x.RealPersonIdentity));
        }

        public void UpdateUserInfo(UserInfo info,bool Append = false)
        {
            if(Append)
            {
                context.UserInfo.Add(info);
            }
            else context.UserInfo.Update(info);
            context.SaveChanges();
        }

    }
}
