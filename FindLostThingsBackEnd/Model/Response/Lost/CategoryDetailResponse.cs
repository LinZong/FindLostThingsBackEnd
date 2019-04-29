using FindLostThingsBackEnd.Persistence.Model;
using System.Linq;

namespace FindLostThingsBackEnd.Model.Response.Lost
{
    public class CategoryDetailResponse : CommonResponse
    {
        public IQueryable<ThingsDetail> CategoryDetails { get; set; }
        public int CategoryId { get; set; }
    }
}
