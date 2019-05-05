using System.Collections.Generic;
using System.Threading.Tasks;
using FindLostThingsBackEnd.Services;
using Jiguang.JPush;
using Jiguang.JPush.Model;

namespace FindLostThingsBackEnd 
{
    public class PushNotifications : IFindLostThingsService
    {
        private readonly JPushClient JClient;
        public PushNotifications(JPushClient jp)
        {
            JClient = jp;
        }
        public Task<HttpResponse> NotifyNewPublsih()
        {
            PushPayload payload = new PushPayload();
            payload.Options = new Options();
            payload.Platform = "android";
            payload.Audience = "all";
            Notification notification = new Notification();
            notification.Android = new Android();
            notification.Android.Alert = "点击进入App内查看";
            notification.Android.Title = "有新的失物信息!";
            Dictionary<string, object> OpenActivity = new Dictionary<string, object>
            {
                { "url", "intent:#Intent;component=misaka.nemesiss.com.findlostthings/misaka.nemesiss.com.findlostthings.SplashActivity;end" }
            };
            notification.Android.Indent = OpenActivity;
            payload.Options.TimeToLive = 60;
            payload.Notification = notification;
            return JClient.SendPushAsync(payload);
        }
    }
}