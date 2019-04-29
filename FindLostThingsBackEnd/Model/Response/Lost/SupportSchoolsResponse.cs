using FindLostThingsBackEnd.Persistence.Model;
using System.Linq;

namespace FindLostThingsBackEnd.Model.Response.Lost
{
    public class SupportSchoolsResponse : CommonResponse
    {
        public IQueryable<SupportSchool> SupportSchools { get; set; }
    }
}
