using ChargeScheduler.Services.User.UIDWorker;
using FindLostThingsBackEnd.Helper;
using FindLostThingsBackEnd.Model.Request.User;
using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Model.Response.User;
using FindLostThingsBackEnd.Persistence.DAO.Operator;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace FindLostThingsBackEnd.Service.User
{
    public class UserServices : IFindLostThingsService
    {
        private readonly UserOperator userOperator;
        private readonly IdWorker worker;
        private readonly ILogger<UserServices> Logger;
        public UserServices(UserOperator user,IdWorker wk,ILogger<UserServices> log)
        {
            userOperator = user;
            worker = wk;
            Logger = log;
        }

        public bool ValidateACTK(long RequestUserID, string CheckACTK)
        {
            var user = userOperator.GetUserInfo(RequestUserID);
            if (user == null) return false;
            else
            {
                if (user.AccessToken != CheckACTK)
                    return false;
                return true;
            }
        }
        public CommonResponse GetUserInfo(long userid)
        {
            var info = userOperator.GetUserInfo(userid);
            if(info == null)
            {
                return new CommonResponse() { StatusCode = 1202 };
            }
            return new UserInfoResponse()
            {
                StatusCode = 0,
                UserInfo = info
            };
        }

        public CommonResponse GetUnAuthenticatedAccountInfo()
        {
            var info = userOperator.GetUnAuthenticatedUser();
            if (info == null)
            {
                return new CommonResponse() { StatusCode = 1202 };
            }
            return new UnAuthenticatedUserResponse()
            {
                StatusCode = 0,
                UserList = info
            };
        }

        public CommonResponse ProcessLoginAccountInfo(LoginUploadAccountInfo info)
        {
            var userInfo = userOperator.GetUserInfo(info.OpenID);
            if(userInfo != null)
            {
                userInfo.AccessToken = info.AccessToken;
                userOperator.UpdateUserInfo(userInfo);
                return new LoginUploadResponse()
                {
                    StatusCode = 0,
                    LastLoginDeviceAndroidID = userInfo.AndroidDevId,
                    UserID = userInfo.Id
                };
            }
            else
            {
                long NewUserID = GenerateUserID();
                userInfo = new UserInfo()
                {
                    Openid = info.OpenID,
                    AccessToken = info.AccessToken,
                    AndroidDevId = info.CurrentDeviceAndroidID,
                    Id = NewUserID,
                    Nickname = info.NickName
                };
                try
                {
                    userOperator.UpdateUserInfo(userInfo,true);
                    return new LoginUploadResponse()
                    {
                        StatusCode = 0,
                        LastLoginDeviceAndroidID = userInfo.AndroidDevId,
                        UserID = userInfo.Id
                    };
                }
                catch (Exception e)
                {
                    ErrorHandler.FormatError<UserServices>(Logger, e);
                    return new CommonResponse()
                    {
                        StatusCode = 1201
                    };
                }
            }
        }
        public CommonResponse UpdateUserInfo(UserInfo info)
        {
            var Info = userOperator.GetUserInfo(info.Id);
            if (Info == null)
            {
                return new CommonResponse()
                {
                    StatusCode = 1202
                };
            }
            else
            {
                try
                {
                    PropertyHelper.DeepCopyProperties<UserInfo,JsonIgnoreAttribute>(info, Info);
                    userOperator.UpdateUserInfo(Info);
                    return new CommonResponse()
                    {
                        StatusCode = 0
                    };
                }
                catch (Exception e)
                {
                    ErrorHandler.FormatError<UserServices>(Logger, e);
                    return new CommonResponse()
                    {
                        StatusCode = 1202
                    };
                }
            }
        }

        public CommonResponse UpdateAccountContacts(AccountContacts cts,long UserID)
        {
            var Info = userOperator.GetUserInfo(UserID);
            if(Info == null)
            {
                return new CommonResponse()
                {
                    StatusCode = 1202
                };
            }
            else
            {
                var UpdatedFields = PropertyHelper.FillNonNullFieldsToUserInfoInstance(cts, Info);
                try
                {
                    userOperator.UpdateUserInfo(Info);
                    return new UpdateAccountContactsResponse()
                    {
                        StatusCode = 0,
                        Updated = UpdatedFields
                    };
                }
                catch (Exception e)
                {
                    ErrorHandler.FormatError<UserServices>(Logger, e);
                    return new CommonResponse()
                    {
                        StatusCode = 1202
                    };
                }
            }
        }


        public long GenerateUserID()
        {
            return worker.NextId();
        }
    }
}
