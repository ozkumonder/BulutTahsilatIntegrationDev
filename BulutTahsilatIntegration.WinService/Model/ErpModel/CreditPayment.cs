using System;
using BulutTahsilatIntegration.WinService.Core;
using Dapper.Contrib.Extensions;

namespace BulutTahsilatIntegration.WinService.Model.ErpModel
{
    public class CreditPayment
    {
        private int _parenRef;
        public int LOGICALREF { get; set; }
        [Computed]
        public string CODE { get; set; }
        [Computed]
        public int BNCRACCREF { get; set; }
        [Computed]
        public int BNCRREF { get; set; }
        public int CREDITREF { get; set; }
        public short PERNR { get; set; }
        public short TRANSTYPE => 1;
        public int PARENTREF
        {
            get { return _parenRef = this.LOGICALREF; }
            set { _parenRef = value; }
        }
        public DateTime DUEDATE { get; set; }
        public DateTime OPRDATE { get; set; }
        public short LINENR { get; set; }
        public double TOTAL { get; set; }
        public double INTTOTAL { get; set; }
        public double BSMVTOTAL { get; set; }
        public double KKDFTOTAL { get; set; }
        public int BNFCHREF { get; set; }
        public short MODIFIED { get; set; }
        public int BNACCREF { get; set; }
        public double TRRATECR { get; set; }
        public double TRRATEACC { get; set; }
        public double EARLYINTRATE { get; set; }
        public double EARLYINTTOT { get; set; }
        public double LATEINTRATE { get; set; }
        public double LATEINTTOT { get; set; }
        public short CAPIBLOCK_CREATEDBY { get; set; }
        public DateTime CAPIBLOCK_CREADEDDATE => DateTime.Now;
        public short CAPIBLOCK_CREATEDHOUR => DateTime.Now.Hour.ToShort();
        public short CAPIBLOCK_CREATEDMIN => DateTime.Now.Minute.ToShort();
        public short CAPIBLOCK_CREATEDSEC => DateTime.Now.Second.ToShort();
        public short CAPIBLOCK_MODIFIEDBY { get; set; }
        public DateTime? CAPIBLOCK_MODIFIEDDATE { get; set; }
        public short CAPIBLOCK_MODIFIEDHOUR { get; set; }
        public short CAPIBLOCK_MODIFIEDMIN { get; set; }
        public short CAPIBLOCK_MODIFIEDSEC { get; set; }
        public short SITEID { get; set; }
        public int ORGLOGICALREF { get; set; }
        public short RECSTATUS { get; set; }
        public int WFSTATUS { get; set; }
        public string LINEEXP { get; set; }
        public double INTRATE { get; set; }
        public double BSMVRATE { get; set; }
        public short FROMCREDITCLOSE { get; set; }
        public string TRANSPECODE { get; set; }
        public string TRANLINEEXP { get; set; }
        public short STRUCTED { get; set; }
        public double SBSMVRATE { get; set; }
        public double SINTRATE { get; set; }
        public int BNCRPARENTREF { get; set; }
    }
}
