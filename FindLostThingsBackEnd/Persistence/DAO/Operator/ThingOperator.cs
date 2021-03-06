﻿using FindLostThingsBackEnd.Helper;
using FindLostThingsBackEnd.Model.Request.Lost;
using FindLostThingsBackEnd.Persistence.DAO.Context;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Persistence.DAO.Operator
{
    public class ThingOperator : IFindLostThingsDbOperator
    {
        private readonly LostContext context;
        private readonly ILogger<ThingOperator> logger;
        public ThingOperator(LostContext ctx, ILogger<ThingOperator> log)
        {
            context = ctx;
            logger = log;
        }

        public IQueryable<LostThingsRecord> GetTimeLines(int HaveFetchItemCount, string EndItemId, int Count = 100)
        {
            //注释掉的是最优解，但是MySQL垃圾引擎不支持。
            var DbTimeLine = context.LostThingsRecord.Where(x => x.Isgiven == 0).OrderByDescending(x => x.PublishTime).Skip(HaveFetchItemCount).Take(Count + 500).ToList();
            if(EndItemId == null || DbTimeLine.All(x => x.Id != EndItemId))
            {
                return DbTimeLine.Take(Count).AsQueryable();
            }
            else
            {
                return DbTimeLine.SkipWhile(x => x.Id != EndItemId).Skip(1).Take(Count).AsQueryable();
            }
        }

        public IQueryable<LostThingsRecord> GetUserPublishedThings(long UserID)
        {
            return context.LostThingsRecord.Where(x => x.Publisher == UserID).OrderByDescending(x => x.PublishTime);
        }

        public IQueryable<LostThingsRecord> GetUserGivenThings(long UserID)
        {
            return context.LostThingsRecord.Where(x => x.Given == UserID).OrderByDescending(x => x.GivenTime);
        }

        public LostThingsRecord GetLostThingsRecord(string RecordGUID)
        {
            return context.LostThingsRecord.FirstOrDefault(x => x.Id == RecordGUID);
        }
        public bool UpdateLostThingRecord(LostThingsRecord Record, bool IsAdd = false)
        {
            using (var tr = context.Database.BeginTransaction())
            {
                try
                {
                    if (IsAdd)
                        context.LostThingsRecord.Add(Record);
                    else
                        context.LostThingsRecord.Update(Record);
                    context.SaveChanges();
                    tr.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    ErrorHandler.FormatError<ThingOperator>(logger, e);
                    tr.Rollback();
                    return false;
                }
            }
        }

        public IQueryable<LostThingsRecord> SearchRecords(SearchLostThingsParameter sp)
        {
            var query = context.LostThingsRecord.Where(x => x.ThingCatId == sp.ThingCatId
                                            && sp.FoundDateBeginUnix <= x.FoundTime 
                                            && x.FoundTime <= sp.FoundDateEndUnix);
            if(sp.ThingDetailId!=null)
            {
                query = query.Where(x => x.ThingDetailId == sp.ThingDetailId);
            }
            string SchoolIdPrefix = sp.SchoolId + "-";
            query = query.Where(x => x.FoundAddress.StartsWith(SchoolIdPrefix));
            if (sp.SchoolBuildingId != null)
            {
                query = query.Where(x => x.FoundAddress == (sp.SchoolId+"-"+sp.SchoolBuildingId).ToString());
            }
            switch (sp.ItemStatus) {
                case 1:
                    query = query.Where(x => x.Isgiven == 0);
                    break;
                case 2:
                    query = query.Where(x => x.Isgiven == 1);
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            switch (sp.SortType)
            {
                case 0:
                    query = query.OrderByDescending(x => x.PublishTime);
                    break;
                case 1:
                    query = query.OrderByDescending(x => x.FoundTime);
                    break;
                default:
                    query = query.OrderByDescending(x => x.PublishTime);
                    break;
            }
            return query;
        }
        private int ExtractSchoolId(string AddressString)
        {
            return int.Parse(AddressString.Split('-')[0]);
        }
        private int ExtractSchoolBuildingId(string AddressString)
        {
            return int.Parse(AddressString.Split('-')[1]);
        }
    }
}
