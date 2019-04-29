using FindLostThingsBackEnd.Persistence.DAO.Context;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Persistence.DAO.Operator
{
    public class ThingsCategoryOperator : IFindLostThingsDbOperator
    {
        private readonly LostContext context;
        public ThingsCategoryOperator(LostContext ctx)
        {
            context = ctx;
        }
        public IQueryable<ThingsCategory> GetThingsCategory()
        {
            return context.ThingsCategory.AsQueryable();
        }

        public ThingsCategory GetThingsCategory(int CategoryId)
        {
            return context.ThingsCategory.First(x => x.Id == CategoryId);
        }
        public IQueryable<ThingsDetail> GetThingsDetail(int CategoryId)
        {
            return context.ThingsDetail.Where(x => x.CategoryId == CategoryId);
        }
        public bool IfCategoryIdExist(int CategoryId)
        {
            return context.ThingsCategory.Count(x => x.Id == CategoryId) > 0;
        }
    }
}
