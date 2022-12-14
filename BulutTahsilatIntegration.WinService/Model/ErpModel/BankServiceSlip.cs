using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.Model.ErpModel
{
    public class BankServiceSlip
    {
        public BankServiceSlip()
        {
            Transactions = new BankServiceSlipTransactions();
            AttachmentInvoice = new AttachmentInvoice();
        }
        [JsonProperty("DataObjectParameter")]
        public DataObjectParameter DataObjectParameter { get; set; }
        [JsonProperty("DATE")]
        public DateTimeOffset? Date { get; set; }

        [JsonProperty("NUMBER")]
        public string Number { get; set; }

        [JsonProperty("TYPE")]
        public int? Type { get; set; }
        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("SIGN")]
        public int? Sign { get; set; }

        [JsonProperty("NOTES1")]
        public string Notes1 { get; set; }

        [JsonProperty("CURRSEL_TOTALS")]
        public int? CurrselTotals { get; set; }
        [JsonProperty("CURRSEL_DETAILS")]
        public int? CurrselDetails { get; set; }

        [JsonProperty("BNACCCODE")]
        public string Bnacccode { get; set; }

        [JsonProperty("TRANSACTIONS")]
        public BankServiceSlipTransactions Transactions { get; set; }

        [JsonProperty("ATTACHMENT_INVOICE")]
        public AttachmentInvoice AttachmentInvoice { get; set; }
    }

    public class BankServiceSlipTransactionItem
    {
        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("TRANNO")]
        public string Tranno { get; set; }

        [JsonProperty("BANKACC_CODE")]
        public string BankaccCode { get; set; }

        [JsonProperty("DATE")]
        public DateTimeOffset? Date { get; set; }

        [JsonProperty("SIGN")]
        public int? Sign { get; set; }

        [JsonProperty("TRCODE")]
        public int? Trcode { get; set; }

        [JsonProperty("DOC_NUMBER")]
        public string DocNumber { get; set; }

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; }

        [JsonProperty("CREDIT")]
        public decimal? Credit { get; set; }

        [JsonProperty("AMOUNT")]
        public decimal? Amount { get; set; }

        [JsonProperty("TC_XRATE")]
        public decimal? TcXrate { get; set; }

        [JsonProperty("TC_AMOUNT")]
        public decimal? TcAmount { get; set; }

        [JsonProperty("RC_XRATE")]
        public decimal? RcXrate { get; set; }
        [JsonProperty("RC_AMOUNT")]
        public decimal? RcAmount { get; set; }
        [JsonProperty("CURR_TRANS")]
        public int? CurrTrans { get; set; }

        [JsonProperty("BANK_PROC_TYPE")]
        public int? BankProcType { get; set; }

        [JsonProperty("DUE_DATE")]
        public DateTimeOffset? DueDate { get; set; }

        [JsonProperty("BN_CRDTYPE")]
        public int? BnCrdtype { get; set; }
    }

    public class BankServiceSlipTransactions
    {
        public BankServiceSlipTransactions()
        {
            items = new List<BankServiceSlipTransactionItem>();
        }
        public List<BankServiceSlipTransactionItem> items { get; set; }
    }

    public class AttachmentInvoice
    {
        public AttachmentInvoice()
        {
            items = new List<AttachmentInvoiceItem>();
        }
        public List<AttachmentInvoiceItem> items { get; set; }
    }

    public class AttachmentInvoiceItem
    {
        public AttachmentInvoiceItem()
        {
            Transactions = new AttachmentInvoiceItemTransactions();
        }
        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("NUMBER")]
        public string Number { get; set; }
        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("DATE")]
        public DateTimeOffset? Date { get; set; }

        [JsonProperty("DOC_DATE")]
        public DateTimeOffset? DocDate { get; set; }

        [JsonProperty("TIME")]
        public int? Time { get; set; }

        [JsonProperty("DOC_NUMBER")]
        public string DocNumber { get; set; }

        [JsonProperty("NOTES1")]
        public string Notes1 { get; set; }

        [JsonProperty("CURR_INVOICE")]
        public int CurrInvoice { get; set; }
        [JsonProperty("TC_XRATE")]
        public decimal? TcXrate { get; set; }

        [JsonProperty("RC_XRATE")]
        public decimal? RcXrate { get; set; }

        [JsonProperty("CURRSEL_TOTALS")]
        public int? CurrselTotals { get; set; }

        [JsonProperty("CURRSEL_DETAILS")]
        public int? CurrselDetails { get; set; }

        [JsonProperty("TRANSACTIONS")]
        public AttachmentInvoiceItemTransactions Transactions { get; set; }
    }

    public class AttachmentInvoiceItemTransactions
    {
        public AttachmentInvoiceItemTransactions()
        {
            items = new List<AttachmentInvoiceTransactionItem>();
        }
        public List<AttachmentInvoiceTransactionItem> items { get; set; }
    }

    public class AttachmentInvoiceTransactionItem
    {
       [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("MASTER_CODE")]
        public string MasterCode { get; set; }
        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("DATE")]
        public DateTimeOffset? Date { get; set; }

        [JsonProperty("FTIME")]
        public int? Time { get; set; }

        [JsonProperty("QUANTITY")]
        public decimal? Quantity { get; set; }

        [JsonProperty("PRICE")]
        public decimal? Price { get; set; }

        [JsonProperty("TOTAL")]
        public decimal? Total { get; set; }

        [JsonProperty("TC_XRATE")]
        public decimal? TcXrate { get; set; }

        [JsonProperty("TC_AMOUNT")]
        public decimal? TcAmount { get; set; }

        [JsonProperty("RC_XRATE")]
        public decimal? RcXrate { get; set; }
        [JsonProperty("RC_AMOUNT")]
        public decimal? RcAmount { get; set; }
        [JsonProperty("CURR_TRANS")]
        public int? CurrTrans { get; set; }

        [JsonProperty("CURRSEL_TRANS")]
        public int? CurrselTrans { get; set; }

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; }

        [JsonProperty("UNIT_CODE")]
        public string UnitCode { get; set; }

        [JsonProperty("EDT_CURR")]
        public int? EdtCurr { get; set; }

        [JsonProperty("EDT_PRICE")]
        public decimal? EdtPrice { get; set; }
    }
}
