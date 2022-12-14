using System;
using Dapper.Contrib.Extensions;

namespace BulutTahsilatIntegration.WinService.Model.Logging
{
    [Table("BTI_TRANSFER_ERRORLOG")]
    public class TransferErrorLog
    {
        public int? LREF { get; set; }
        public int? PAYMENTID { get; set; }
        public DateTime PROCDATE { get; set; }
        public DateTime FICHEDATE { get; set; }
        public string PAYMENTEXPCODE { get; set; }
        public string BULUTFICHETYPE { get; set; }
        public string LOGOFICHETYPE { get; set; }
        public string SENDERFIRMNAME { get; set; }
        public string SENDERBANK { get; set; }
        public string SENDERIBAN { get; set; }
        public string BRANCHFIRMNAME { get; set; }
        public string FIRMBANK { get; set; }
        public string FIRMIBAN { get; set; }
        public string EXPLANATION { get; set; }
        public string PAYMENTDATA { get; set; }
        public string ERROR { get; set; }
        public string ERPTRANSFEREXP { get; set; }
        public string ERPFICHENO { get; set; }
        public string ERPRESPONSE { get; set; }
        public string ERPPOSTJSON { get; set; }
        public string STATUSINFO { get; set; }
    }
}
