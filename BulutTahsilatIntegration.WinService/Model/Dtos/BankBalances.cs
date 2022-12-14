using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Dapper;
using DapperExtensions.Mapper;

namespace BulutTahsilatIntegration.WinService.Model.Dtos
{
    [Table("BTI_BULUTBANKBALANCE")]
    public class BankBalances
    {

        public string BANKCODE { get; set; }

        /// <remarks/>
        public string BANKNAME { get; set; }
        public string FIRMNAME { get; set; }

        /// <remarks/>
        public string BANKBRANCHNAME { get; set; }

        /// <remarks/>
        public string BANKNICKNAME { get; set; }

        /// <remarks/>
        public string BANKACCOUNTTYPE { get; set; }

        /// <remarks/>
        public string BANKIBAN { get; set; }

        /// <remarks/>
        public string BANKCURRENCYUNIT { get; set; }

        /// <remarks/>
        public DateTime? LASTTIMESTMAP { get; set; }

        /// <remarks/>
        public decimal? BALANCE { get; set; }

        /// <remarks/>
        public decimal? BLOCKEDBALANCE { get; set; }


    }
}
