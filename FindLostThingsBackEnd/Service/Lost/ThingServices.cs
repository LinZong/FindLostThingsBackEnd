using FindLostThingsBackEnd.Helper;
using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Persistence.DAO.Operator;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using System.Linq;

namespace FindLostThingsBackEnd.Service.Lost
{
    public enum UserThingsListType
    {
        Published,
        Given
    }

    public class ThingServices : IFindLostThingsService
    {
        private readonly ThingOperator thing;
        public ThingServices(ThingOperator th)
        {
            thing = th;
        }

        public IQueryable<LostThingsRecord> GetTimeLines(int HaveFetchedCount, string EndItemId, int Count)
        {
            return thing.GetTimeLines(HaveFetchedCount, EndItemId, Count);
        }

        public IQueryable<LostThingsRecord> GetUserThings(long UserID, UserThingsListType type)
        {
            switch (type)
            {
                case UserThingsListType.Published:
                    return thing.GetUserPublishedThings(UserID);
                case UserThingsListType.Given:
                    return thing.GetUserGivenThings(UserID);
                default:
                    return thing.GetUserPublishedThings(UserID);
            }
        }

        public CommonResponse UpdateLostThingRecord(LostThingsRecord ChangedRecord)
        {
            if (ChangedRecord == null)
            {
                return new CommonResponse()
                {
                    StatusCode = 1504
                };
            }
            // 首先检查待更新的对象在数据库中是否存在旧对象
            var RecordGUID = ChangedRecord.Id;
            var OldRecord = thing.GetLostThingsRecord(RecordGUID);
            if (OldRecord == null)
            {
                return new CommonResponse()
                {
                    StatusCode = 1501
                };
            }
            else
            {
                // 然后检查新对象是否修改了不应该修改的Field。
                bool UnchangedValidation = ThingHelper.EnsureNotModifyFieldsUnchanged(OldRecord, ChangedRecord);
                if (!UnchangedValidation)
                {
                    return new CommonResponse()
                    {
                        StatusCode = 1502
                    };
                }
                else
                {
                    // 校验成功之后，合并新旧对象的Fields，向数据库中写入。
                    OldRecord.MergeLostThingsRecord(ChangedRecord);
                    if (thing.UpdateLostThingRecord(OldRecord))
                        return new CommonResponse()
                        {
                            StatusCode = 0
                        };
                    else
                        return new CommonResponse()
                        {
                            StatusCode = 1503
                        };
                }
            }
        }

        public CommonResponse PublishLostThingRecord(LostThingsRecord record)
        {
            if (record == null)
            {
                return new CommonResponse()
                {
                    StatusCode = 1505
                };
            }
            if (thing.UpdateLostThingRecord(record,true))
            {
                return new CommonResponse()
                {
                    StatusCode = 0
                };
            }
            else
            {
                return new CommonResponse()
                {
                    StatusCode = 1503
                };
            }
        }
    }
}
