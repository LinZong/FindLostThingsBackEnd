using Newtonsoft.Json;

namespace FindLostThingsBackEnd.Persistence.Model
{
    public partial class ThingsDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }
}
