﻿using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FindLostThingsBackEnd.Service.Tencent
{
    public class TencentCosTempKey
    {
        private static string StsDomain = "sts.tencentcloudapi.com";
        private static string RequestStsUrl = "https://sts.tencentcloudapi.com/";
        public string SecretId { get; set; }
        public string SecretKey { get; set; }
        public int DurationSeconds { get; set; }
        public string BucketName { get; set; }
        public string AppID { get; set; }

        public string Region { get; set; }
        public string AllowPrefix { get; set; }

        private static Random RandInstance = new Random();

        private static RestClient client = new RestClient(RequestStsUrl);


        private static string ToQueryString(List<KeyValuePair<string, string>> Params, bool ShouldURIEncode = false)
        {
            List<string> Combine = new List<string>();
            foreach (var i in Params)
            {
                string value = ShouldURIEncode ? WebUtility.UrlEncode(i.Value) : i.Value;
                Combine.Add($"{i.Key}={value}");
            }
            return string.Join("&", Combine.ToArray());
        }
        private static int GetRandom()
        {
            return RandInstance.Next(10000, 20000);
        }
        private static string GetSignature(List<KeyValuePair<string, string>> opt, string key, string method)
        {
            var formatString = method + StsDomain + "/?" + ToQueryString(opt);
            return ToBase64hmac(formatString, key);
        }

        public static string ToBase64hmac(string strText, string strKey)
        {
            HMACSHA1 myHMACSHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(strKey));
            byte[] byteText = myHMACSHA1.ComputeHash(Encoding.UTF8.GetBytes(strText));
            return Convert.ToBase64String(byteText);
        }

        public IRestResponse GetSignature()
        {
            var secretId = SecretId;
            var secretKey = SecretKey;
            var host = RequestStsUrl;
            var durationSeconds = DurationSeconds;
            var policyStr = GetFormattedPolicy(Region, AppID, BucketName, AllowPrefix);
            var action = "GetFederationToken";
            var nonce = GetRandom();
            var timestamp = DateTime2Unix(DateTime.Now);
            var method = "POST";

            List<KeyValuePair<string, string>> par = new List<KeyValuePair<string, string>>();


            par.Add(new KeyValuePair<string, string>("SecretId", secretId));
            par.Add(new KeyValuePair<string, string>("Timestamp", timestamp.ToString()));
            par.Add(new KeyValuePair<string, string>("Nonce", nonce.ToString()));
            par.Add(new KeyValuePair<string, string>("Action", action));
            par.Add(new KeyValuePair<string, string>("DurationSeconds", durationSeconds.ToString()));
            par.Add(new KeyValuePair<string, string>("Name", "cos-sts-nodejs"));
            par.Add(new KeyValuePair<string, string>("Version", "2018-08-13"));
            par.Add(new KeyValuePair<string, string>("Region", "ap-guangzhou"));
            par.Add(new KeyValuePair<string, string>("Policy", policyStr));
            par.Sort(CompareKey);
            var sig = GetSignature(par, secretKey, method);
            par.Add(new KeyValuePair<string, string>("Signature", sig));

            RestRequest req = new RestRequest();
            req.Method = Method.POST;
            req.Resource = RequestStsUrl;
            req.AddHeader("Host", StsDomain);
            foreach (var i in par)
            {
                req.AddParameter(i.Key, i.Value, ParameterType.GetOrPost);
            }
            return client.Execute(req);
        }

        private static int CompareKey(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
        {
            return x.Key.CompareTo(y.Key);
        }

        public static long DateTime2Unix(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static string GetFormattedPolicy(string region, string appId, string shortBucketName, string allowPrefix)
        {
            return "%7b%22version%22%3a%222.0%22%2c%22statement%22%3a%5b%7b%22action%22%3a%5b%22name%2fcos%3aPutObject%22%2c%22name%2fcos%3aPostObject%22%2c%22name%2fcos%3aInitiateMultipartUpload%22%2c%22name%2fcos%3aListMultipartUploads%22%2c%22name%2fcos%3aListParts%22%2c%22name%2fcos%3aUploadPart%22%2c%22name%2fcos%3aCompleteMultipartUpload%22%2c%22name%2fcos%3aHeadObject%22%2c%22name%2fcos%3aGetObject%22%2c%22name%2fcos%3aGetObjectACL%22%5d%2c%22effect%22%3a%22allow%22%2c%22resource%22%3a%5b%22qcs%3a%3acos%3aap-guangzhou%3auid%2f1255798866%3anemesiss-1255798866%2f*%22%5d%7d%5d%7d";
        }
    }

    public static class TencentCosDependencyInjection
    {
        public static IServiceCollection AddTencentCos(this IServiceCollection service,Action<TencentCosTempKey> configuration)
        {
            var instance = new TencentCosTempKey();
            configuration(instance);
            return service.AddScoped(_ => instance);
        }
    }
}
