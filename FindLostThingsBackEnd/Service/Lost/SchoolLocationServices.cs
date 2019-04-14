using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Model.Response.Lost;
using FindLostThingsBackEnd.Persistence.DAO.Operator;
using FindLostThingsBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Service.Lost
{
    public class SchoolLocationServices : IFindLostThingsService
    {
        private readonly SchoolOperator school;
        public SchoolLocationServices(SchoolOperator sc)
        {
            school = sc;
        }

        public CommonResponse GetSupportSchools()
        {
            return new SupportSchoolsResponse()
            {
                StatusCode = 0,
                SupportSchools = school.GetSupportSchools()
            };
        }

        public CommonResponse GetSchoolBuildingInfo(int SchoolId)
        {
            int SchoolRecord = school.GetSupportSchools().Count(x => x.Id == SchoolId);
            if(SchoolRecord <= 0)
            {
                return new CommonResponse()
                {
                    StatusCode = 1401
                };
            }
            else
            {
                return new SchoolBuildingsResponse()
                {
                    StatusCode = 0,
                    SchoolBuildings = school.GetSchoolBuildingInfo(SchoolId)
                };
            }
        }
    }
}
