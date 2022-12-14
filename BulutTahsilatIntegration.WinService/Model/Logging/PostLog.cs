using System;

namespace BulutTahsilatIntegration.WinService.Model.Logging
{
    public class PostLog
    {
        public int? Id { get; set; }
        public string HostIp { get; set; }
        public DateTime PostDate { get; set; }
        public string IdentityName { get; set; }
        public string OperationType { get; set; }
        public string Url { get; set; }
        public string RequestMethod { get; set; }
        public string JsonData { get; set; }
    }
}