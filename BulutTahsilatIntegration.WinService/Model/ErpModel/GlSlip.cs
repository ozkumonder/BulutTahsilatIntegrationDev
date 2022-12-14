using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.Model.ErpModel
{
    public class GlSlip
    {
        public GlSlip()
        {
            Transactions = new TRANSACTIONS();
        }
        public DataObjectParameter DataObjectParameter { get; set; }

        [JsonProperty("TYPE")]
        public int Type;

        [JsonProperty("NUMBER")]
        public string Number;

        [JsonProperty("DATE")]
        public DateTime Date;

        [JsonProperty("AUXIL_CODE")]
        public string AuxilCode;

        [JsonProperty("NOTES1")]
        public string Notes1;

        [JsonProperty("CURRSEL_TOTALS")]
        public int CurrselTotals;

        [JsonProperty("CURRSEL_DETAILS")]
        public int CurrselDetails;

        [JsonProperty("TRANSACTIONS")]
        public TRANSACTIONS Transactions;

        [JsonProperty("DOC_DATE")]
        public DateTime DocDate;

        [JsonProperty("EBOOK_NODOCUMENT")]
        public int EBookNoDocument;

        [JsonProperty("EBOOK_DOCTYPE")]
        public int EBookDocType;
    }
    public class GlSlipTransactionItem
    {
        [JsonProperty("DATE")]
        public DateTime Date;

        [JsonProperty("GL_CODE")]
        public string GlCode;

        [JsonProperty("SIGN")]
        public int? Sign;

        [JsonProperty("CREDIT")]
        public double? Credit;

        [JsonProperty("DEBIT")]
        public double? Debit;

        [JsonProperty("DESCRIPTION")]
        public string Description;

        [JsonProperty("CURR_TRANS")]
        public int CurrTrans;

        [JsonProperty("RC_XRATE")]
        public double RcXrate;

        [JsonProperty("RC_AMOUNT")]
        public double RcAmount;

        [JsonProperty("TC_XRATE")]
        public double TcXrate;

        [JsonProperty("TC_AMOUNT")]
        public double TcAmount;

        [JsonProperty("CURRSEL_TRANS")]
        public int CurrselTrans;

        [JsonProperty("DOC_DATE")]
        public DateTime DocDate;

        [JsonProperty("LINE_TYPE")]
        public int LineType;

        [JsonProperty("CODE_REF")]
        public int CodeRef;

    }

    public class TRANSACTIONS
    {
        public TRANSACTIONS()
        {
            items = new List<GlSlipTransactionItem>();
        }
        [JsonProperty("items")]
        public IList<GlSlipTransactionItem> items { get; set; }
    }

}
