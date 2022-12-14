using System;
using System.Collections.Generic;
using BulutTahsilatIntegration.WinService.Model;
using Newtonsoft.Json;

namespace BulutTahsilatintegration.WinService.Model.ErpModel
{
    public class CheckSlip
    {
        public CheckSlip()
        {
            Transactions = new Transactions();
            BankTransactions = new BankTransactions();
        }

        [JsonProperty("DataObjectParameter")]
        public DataObjectParameter DataObjectParameter { get; set; }

        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("NUMBER")]
        public string Number { get; set; }

        [JsonProperty("MASTER_CODE")]
        public string MasterCode { get; set; }

        [JsonProperty("CARDREF")]
        public int? CardRef { get; set; }

        [JsonProperty("DATE")]
        public DateTime? Date { get; set; }

        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("CYPHCODE")]
        public string Cyphcode { get; set; }

        [JsonProperty("DOC_NUMBER")]
        public string DocNumber { get; set; }

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; }

        [JsonProperty("PROC_TYPE")]
        public int? ProcType { get; set; }
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

        [JsonProperty("BANK_TRANSACTIONS")]
        public BankTransactions BankTransactions { get; set; }

        [JsonProperty("AFFECT_RISK")]
        public int? AffectRisk { get; set; }
        [JsonProperty("DEG_ACTIVE")]
        public int? DegActive { get; set; }
        [JsonProperty("DEG_CURR")]
        public int? DegCurr { get; set; }

        [JsonProperty("EBOOK_DOCTYPE")]
        public int? EbookDocType { get; set; }
    }
    public class Transactions
    {
        public Transactions()
        {
            items = new List<TransactionItem>();
        }

        [JsonProperty("items")]
        public IList<TransactionItem> items { get; set; }
    }
    public class TransactionItem
    {
        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("CURRENT_STATUS")]
        public int? CurrentStatus { get; set; }

        [JsonProperty("BANK_CODE")]
        public string BankCode { get; set; }
        [JsonProperty("ACCOUNTREF")]
        public int? AccountRef { get; set; }

        [JsonProperty("BNACCOUNTREF")]
        public int? BnAccountRef { get; set; }

        [JsonProperty("NUMBER")]
        public string Number { get; set; }

        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode { get; set; }

        [JsonProperty("AUTH_CODE")]
        public string AuthCode { get; set; }

        [JsonProperty("DUE_DATE")]
        public DateTime DueDate { get; set; }

        [JsonProperty("DATE")]
        public DateTime Date { get; set; }

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

        [JsonProperty("CURRSEL_TRANS")]
        public int? CurrselTrans { get; set; }

        [JsonProperty("CSREF")]
        public int? CsRef { get; set; }

        [JsonProperty("ROLLREF")]
        public int? RollRef { get; set; }

        [JsonProperty("TRANS_STATUS")]
        public int? TransStatus { get; set; }

        [JsonProperty("CARDMD")]
        public int? CardMd { get; set; }

        [JsonProperty("CARDREF")]
        public int? CardRef { get; set; }

        [JsonProperty("SERIAL_NR")]
        public string SerialNr { get; set; }

        [JsonProperty("AFFECT_RISK")]
        public int? AffectRisk { get; set; }

        [JsonProperty("TAX_NR")]
        public string TaxNr { get; set; }

        [JsonProperty("CS_IBAN")]
        public string CsIban { get; set; }
    }
    public class BankTransactions
    {
        public BankTransactions()
        {
            items = new List<BankTransactionItem>();
        }
        [JsonProperty("items")]
        public IList<BankTransactionItem> items { get; set; }
    }
    public class BankTransactionItem
    {

        [JsonProperty("TYPE")]
        public int? Type { get; set; }

        [JsonProperty("TRANNO")]
        public string TranNo { get; set; }

        [JsonProperty("BANKACC_CODE")]
        public string BankAccCode { get; set; }

        [JsonProperty("ACCOUNTREF")]
        public int? AccountRef { get; set; }

        [JsonProperty("BNACCOUNTREF")]
        public int? BnAccountRef { get; set; }

        [JsonProperty("DATE")]
        public DateTime Date { get; set; }

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; }
        [JsonProperty("SIGN")]
        public int? Sign { get; set; }
        

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

        [JsonProperty("CURRSEL_TRANS")]
        public int? CurrselTrans { get; set; }

        [JsonProperty("DUE_DATE")]
        public DateTime DueDate { get; set; }

        [JsonProperty("GRPFIRMTRANS")]
        public int? GrpFirmTrans { get; set; }

        [JsonProperty("BN_CRDTYPE")]
        public int? BnCrdType { get; set; }
    }





}
