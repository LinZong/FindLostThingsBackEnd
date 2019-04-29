using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Model.Response.Lost;
using FindLostThingsBackEnd.Persistence.DAO.Operator;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Service.Lost
{
    public class CategoryServices : IFindLostThingsService
    {
        private readonly ThingsCategoryOperator thingsCategory;
        public CategoryServices(ThingsCategoryOperator tco)
        {
            thingsCategory = tco;
        }

        public CategoryResponse GetThingsCategory()
        {
            var CategoryQuery = thingsCategory.GetThingsCategory();
            return new CategoryResponse() { StatusCode = 0, CategoryList = CategoryQuery };
        }
        public CommonResponse GetThingsCategoryDetail(int CategoryId)
        {
            if(!thingsCategory.IfCategoryIdExist(CategoryId))
            {
                return new CommonResponse() { StatusCode = 1101 };
            }
            return new CategoryDetailResponse() { StatusCode = 0,
                                                  CategoryId = CategoryId,
                                                  CategoryDetails = thingsCategory.GetThingsDetail(CategoryId)
                                                };
        }
    }
}
