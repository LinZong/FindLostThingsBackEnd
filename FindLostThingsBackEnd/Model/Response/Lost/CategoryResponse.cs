using FindLostThingsBackEnd.Persistence.Model;
using System.Linq;

namespace FindLostThingsBackEnd.Model.Response.Lost
{
    public class CategoryResponse : CommonResponse
    {
        public IQueryable<ThingsCategory> CategoryList { get; set; }
    }
}
