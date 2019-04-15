using FindLostThingsBackEnd.Persistence.DAO.Context;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FindLostThingsBackEnd.Persistence.DAO.Operator
{
    public class SchoolOperator : IFindLostThingsDbOperator
    {
        private readonly LostContext context;
        public SchoolOperator(LostContext ctx)
        {
            context = ctx;
        }

        public IQueryable<SupportSchool> GetSupportSchools()
        {
            return context.SupportSchool.AsQueryable();
        }

        public IQueryable<SchoolBuildingInfo> GetSchoolBuildingInfo(int SchoolId)
        {
            var school = GetSupportSchools().First(x => x.Id == SchoolId);
            var SchoolTbName = school.SchoolAddrTbName;
            string FmtQuery = $"SELECT * FROM lost.{SchoolTbName};";
            return context.SchoolInfo.FromSql(FmtQuery);
        }
    }
}
