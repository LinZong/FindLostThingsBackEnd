namespace FindLostThingsBackEnd.Model.Response.Lost
{
    public class TencentCosTempKeyResponse : CommonResponse
    {
        public string FullBucketName { get; set; }
        public string Region { get; set; }

        public string Response { get; set; }

    }
}
