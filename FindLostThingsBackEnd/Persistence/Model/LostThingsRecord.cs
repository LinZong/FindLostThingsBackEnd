using FindLostThingsBackEnd.Middleware;
using System;
using System.Collections.Generic;

namespace FindLostThingsBackEnd.Persistence.Model
{
    public partial class LostThingsRecord
    {
        [ShouldNotModify]
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThingCatId { get; set; }
        public int ThingDetailId { get; set; }
        [ShouldNotModify]
        public long PublishTime { get; set; }
        public long FoundTime { get; set; }
        public long? GivenTime { get; set; }
        public int? Isgiven { get; set; }
        public string FoundAddress { get; set; }
        public string FoundAddrDescription { get; set; }
        public string ThingAddiDescription { get; set; }
        public string ThingPhotoUrls { get; set; }
        public string PublisherContacts { get; set; }
        public string GivenContacts { get; set; }
        [ShouldNotModify]
        public long Publisher { get; set; }
        public long? Given { get; set; }
    }
}
