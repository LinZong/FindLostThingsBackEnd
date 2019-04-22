using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Model.Request.Lost
{
    public class SearchLostThingsParameter
    {
        public int ThingCatId { get; set; }
        public int? ThingDetailId { get; set; }
        public int SchoolId { get; set; }
        public int? SchoolBuildingId { get; set; }
        public int ItemStatus { get; set; } //01 未完成，10已完成，11未完成+已完成
        public int SortType { get; set; } //按照发布时间排序0, 按照捡到失物的时间排序1
        public int IsAdvancedSort { get; set; }

        public long FoundDateBeginUnix { get; set; }
        public long FoundDateEndUnix { get; set; }


        public string AdvancedSortText { get; set; } //这个可能需要动用到搜索引擎的分词技术，先标记为正在开发...

    }
}
