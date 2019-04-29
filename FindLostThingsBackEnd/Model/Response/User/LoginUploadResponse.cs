namespace FindLostThingsBackEnd.Model.Response.User
{
    public class LoginUploadResponse : CommonResponse
    {
        public long UserID { get; set; }
        public string LastLoginDeviceAndroidID { get; set; }
    }
}
