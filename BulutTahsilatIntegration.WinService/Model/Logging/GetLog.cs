using System;

namespace BulutTahsilatIntegration.WinService.Model.Logging
{
    public class GetLog
    {
        public int Id { get; set; }
        public DateTime ProcDate { get; set; }
        public string OperationName { get; set; }
        public int PaymentStatusTypeId { get; set; }
        public DateTime BegDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MethodName { get; set; }
        public string Filters { get; set; }
        public string RawData { get; set; }
        public string FilteredData { get; set; }
    }
}
