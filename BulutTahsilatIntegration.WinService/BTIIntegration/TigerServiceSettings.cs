namespace BulutTahsilatIntegration.WinService.BTIIntegration
{
    public class TigerServiceSettings
    {
        public static  string TigerRestServiceName => "LogoLObjectRestService";
        public string ServiceUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public bool Compressed { get; set; }


        public string FirmCode { get; set; }
        public int? MaxConnectionCount { get; set; }

        public string Password { get; set; }

        public string PeriodCode { get; set; }

        public string UserName { get; set; }

        public TigerServiceSettings()
        {
        }
    }
}