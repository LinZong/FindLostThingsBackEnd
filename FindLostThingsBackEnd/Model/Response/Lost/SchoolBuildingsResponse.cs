using FindLostThingsBackEnd.Persistence.Model;
using System.Linq;

namespace FindLostThingsBackEnd.Model.Response.Lost
{
    public class SchoolBuildingsResponse : CommonResponse
    {
        public IQueryable<SchoolBuildingInfo> SchoolBuildings { get; set; }
    }
}
