namespace FindLostThingsBackEnd.Model.Request.User
{
    public class LoginUploadAccountInfo
    {
        public string OpenID { get; set; }
        public string AccessToken { get; set; }
        public string NickName { get; set; }
        public string CurrentDeviceAndroidID { get; set; }

    }
}
