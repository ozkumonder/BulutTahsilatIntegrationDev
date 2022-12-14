using System;
using BulutTahsilatIntegration.WinService.Core;
using Dapper.Contrib.Extensions;

namespace BulutTahsilatIntegration.WinService.Model.ErpModel
{
    public class LeasingPaymet
    {
        public int LOGICALREF { get; set; }
        public int FICHEREF { get; set; }
        public short LINENR => 1;
        public DateTime PAYMENTDATE { get; set; }
        public double PAYMENTTOTAL { get; set; }
        public double INTTOTAL { get; set; }
        public double MAINTOTAL { get; set; }
        public double MAINREMAINED { get; set; }
        public short TRCURR { get; set; }
        public double TRRATE { get; set; }
        public short BOSTATUS { get; set; }
        public double VATINPAYMENTTOTAL { get; set; }
        public short PAYMENTTYPE { get; set; }
        public short SITEID { get; set; }
        public short RECSTATUS { get; set; }
        public int ORGLOGICREF { get; set; }
        public int WFSTATUS { get; set; }
        public short CAPIBLOCK_CREATEDBY { get; set; }
        public DateTime CAPIBLOCK_CREADEDDATE => DateTime.Now;
        public short CAPIBLOCK_CREATEDHOUR => DateTime.Now.Hour.ToShort();
        public short CAPIBLOCK_CREATEDMIN => DateTime.Now.Minute.ToShort();
        public short CAPIBLOCK_CREATEDSEC => DateTime.Now.Second.ToShort();
        public short CAPIBLOCK_MODIFIEDBY { get; set; }
        public DateTime CAPIBLOCK_MODIFIEDDATE { get; set; }
        public short CAPIBLOCK_MODIFIEDHOUR { get; set; }
        public short CAPIBLOCK_MODIFIEDMIN { get; set; }
        public short CAPIBLOCK_MODIFIEDSEC { get; set; }
        public int BNACCREF { get; set; }
        public int LEASINGREF { get; set; }
        public short TRANSTYPE => 1;
        private int _parenRef;

        public int PARENTREF
        {
            get { return _parenRef = this.LOGICALREF; }
            set { _parenRef = value; }
        }
        public int BNFCHREF { get; set; }
        public double DEPRECIATION { get; set; }
        public double PROFITLOSS { get; set; }
        public int ACTACCREF { get; set; }
        public double RENTOBL { get; set; }
        public double INTRATE { get; set; }
        public double NPVTODAYVAL { get; set; }
        public double TENANCY { get; set; }
        [Computed]
        public virtual string CODE { get; set; }
    }
}
