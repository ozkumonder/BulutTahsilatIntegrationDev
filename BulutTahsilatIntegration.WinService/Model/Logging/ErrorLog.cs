using System;

namespace BulutTahsilatIntegration.WinService.Model.Logging
{
    public class ErrorLog
    {
        public int? Id { get; set; }
        public int? PostId { get; set; }
        public string HostIp { get; set; }
        public DateTime PostDate { get; set; }
        public string IdentityName { get; set; }
        public string OperationType { get; set; }
        public string ErrorClassName { get; set; }
        public string ErrorMethodName { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerException { get; set; }
        public string JsonData { get; set; }
        public string ResponseData { get; set; }
    }
}
