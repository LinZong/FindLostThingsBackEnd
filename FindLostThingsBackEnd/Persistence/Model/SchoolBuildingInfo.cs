using System;
using System.Collections.Generic;

namespace FindLostThingsBackEnd.Persistence.Model
{
    public partial class SchoolBuildingInfo
    {
        public int Id { get; set; }
        public string BuildingName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string BuildingAddress { get; set; }
    }
}
