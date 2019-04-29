using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FindLostThingsBackEnd.Persistence.Model
{
    public partial class SupportSchool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string SchoolAddrTbName { get; set; }
    }
}
