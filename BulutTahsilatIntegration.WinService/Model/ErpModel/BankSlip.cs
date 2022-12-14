using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.Model.ErpModel
{
    public class BankSlip
    {
        public BankSlip()
        {
            Transactions = new Transactions();
        }

        public DataObjectParameter DataObjectParameter { get; set; }

        /// <summary>
        /// Kayıt Referansı
        /// </summary>
        [JsonProperty("INTERNAL_REFERENCE")]
        public int? InternalReference { get; set; }

        /// <summary>
        /// Tarih
        /// </summary>
        [JsonProperty("DATE")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Fiş No
        /// </summary>
        [JsonProperty("NUMBER")]
        public string Number { get; set; }
        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("AUTH_CODE")]
        public string AuthCode { get; set; }

        [JsonProperty("DIVISION")]
        public int? Division { get; set; }

        [JsonProperty("DEPARMENT")]
        public int? Deparment { get; set; }

        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("SIGN")]
        public int? Sign { get; set; }

        [JsonProperty("NOTES1")]
        public string Notes1 { get; set; }

        [JsonProperty("NOTES2")]
        public string Notes2 { get; set; }
        [JsonProperty("CURRSEL_TOTALS")]
        public int? CurrselTotals { get; set; }

        [JsonProperty("CURRSEL_DETAILS")]

        public int? CurrselDetails { get; set; }

        [JsonProperty("TRANSACTIONS")]
        public Transactions Transactions { get; set; }

        [JsonProperty("EBOOK_DOCTYPE")]
        public long EbookDoctype { get; set; }

        [JsonProperty("BNCREREF")]
        public long Bncreref { get; set; }

        [JsonProperty("BANK_CREDIT_CODE")]
        public string BankCreditCode { get; set; }
    }

    public partial class Transaction
    {
        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("TRANNO")]
        public string Tranno { get; set; }
        [JsonProperty("BANKREF")]
        public int? BankRef { get; set; }

        [JsonProperty("BANKACC_CODE")]
        public string BankaccCode { get; set; }

        [JsonProperty("ARP_CODE")]
        public string ArpCode { get; set; }

        [JsonProperty("DATE")]
        public DateTime? Date { get; set; }

        [JsonProperty("DEPARTMENT")]
        public long? Department { get; set; }

        [JsonProperty("BRANCH")]
        public long? Branch { get; set; }

        [JsonProperty("SIGN")]
        public long? Sign { get; set; }

        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("CYPHCODE")]
        public string Cyphcode { get; set; }

        [JsonProperty("DOC_NUMBER")]
        public string DocNumber { get; set; }

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; }

        [JsonProperty("CURR_TRANS")]
        public int? CurrTrans { get; set; }

        [JsonProperty("DEBIT")]
        public decimal? Debit { get; set; }

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

        [JsonProperty("ARP_BNKDIV_NR")]
        public string ArpBnkdivNr { get; set; }

        [JsonProperty("ARP_BNKACCOUNT_NR")]
        public string ArpBnkaccountNr { get; set; }

        [JsonProperty("BNK_TRACKING_NR")]
        public string BnkTrackingNr { get; set; }

        [JsonProperty("TRADING_GRP")]
        public string TradingGrp { get; set; }

        [JsonProperty("CURRSEL_TRANS")]
        public int? CurrselTrans { get; set; }

        [JsonProperty("BANK_PROC_TYPE")]
        public int? BankProcType { get; set; }

        [JsonProperty("DUE_DATE")]
        public DateTime DueDate { get; set; }

        [JsonProperty("PROJECT_CODE")]
        public string ProjectCode { get; set; }

        [JsonProperty("AFFECT_RISK")]
        public byte AffectRisk { get; set; }

        [JsonProperty("BANK_BRANCHS")]
        public string BankBranchs { get; set; }

        [JsonProperty("SALESMAN_CODE")]
        public string SalesmanCode { get; set; }

        [JsonProperty("IBAN")]
        public string Iban { get; set; }
        
        [JsonProperty("BN_CRDTYPE")]
        public int? BnCrdtype { get; set; }

        [JsonProperty("DIVISION")]
        public int? Division { get; set; }
    }

    public class Transactions
    {
        public Transactions()
        {
            items = new List<Transaction>();
        }
        public List<Transaction> items { get; set; }
    }

}
