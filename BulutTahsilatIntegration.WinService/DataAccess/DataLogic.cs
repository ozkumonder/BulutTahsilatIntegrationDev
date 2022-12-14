using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.Model;
using BulutTahsilatIntegration.WinService.Model.Dtos;
using BulutTahsilatIntegration.WinService.Model.ErpModel;
using BulutTahsilatIntegration.WinService.Model.Global;
using BulutTahsilatIntegration.WinService.Model.Logging;
using BulutTahsilatIntegration.WinService.Model.ResultTypes;
using BulutTahsilatIntegration.WinService.Utilities;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.DataAccess
{
    public class DataLogic
    {
        private static readonly string error;
        private static readonly WinService.DataAccess.DataAccess dataAccess = new DataAccess(ConfigHelper.ConnectionString(), ref error);
        private static DataAccessException exception;
        private static QueriesConfig SqlQuery { get; } = ConfigHelper.DeserializeXml<QueriesConfig>(ConfigHelper.ReadQueriesPath);

        #region Firma İşlemleri

        public static int GetReportCurrencyType(int firmNo)
        {
            var result = 0;
            try
            {
                var sql = "SELECT FIRMREPCURR FROM L_CAPIFIRM WHERE NR = @NR".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { NR = firmNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }

        #endregion

        #region Client Operation
        /// <summary>
        /// Bulut tahsilat servisine gönderilecek cari listesini getirir.
        /// </summary>
        /// <returns></returns>
        public static List<ClientCard> GetClients()
        {
            var result = new List<ClientCard>();
            try
            {
                //var sql = @"SELECT CLC.LOGICALREF AS 'Id', CLC.CODE AS 'ClientCode', CLC.DEFINITION_ AS 'FirmName',CLC.EDINO AS 'PaymentExpCode', CLC.ADDR1 +' '+ CLC.ADDR2 AS 'Address', CLC.TOWN as 'County',  34  AS 'CityID',CASE CLC.ISPERSCOMP WHEN 1 THEN CLC.TCKNO ELSE  CLC.TAXNR END AS 'TaxNumber',CLC.TAXOFFICE AS 'TaxOffice'
                //               --,ISNULL((SELECT CODE FROM LG_XXX_EMUHACC WHERE LOGICALREF = (SELECT TOP 1 ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TYP = 1 AND TRCODE = 5 AND CARDREF = CLC.LOGICALREF)),'') AS    'AccountingCode'
                //               FROM LG_XXX_CLCARD CLC WITH(NOLOCK)
                //               WHERE CLC.ACTIVE = 0 
                //               AND CLC.CARDTYPE NOT IN(4,22) 
                //               AND CLC.BANKIBANS1 <> '' 
                //               AND CLC.EDINO = ''
                //               AND CLC.TAXOFFICE <> '' 
                //               AND CLC.CITYCODE <> ''
                //               AND CLC.CITYCODE LIKE '%[0-99]%'                                  
                //               AND (LEN(CLC.TCKNO) < 11 OR LEN(CLC.TAXNR) < 10) 
                //               AND (CLC.CODE LIKE '120%' OR CLC.CODE LIKE '320%' OR CLC.CODE LIKE '195%')
                //               UNION ALL
                //               SELECT CLC.LOGICALREF AS 'Id', CLC.CODE AS 'ClientCode', CLC.DEFINITION_ AS 'FirmName',CLC.EDINO AS 'PaymentExpCode', CLC.ADDR1 +' '+ CLC.ADDR2 AS 'Address', CLC.TOWN as 'County',  34  AS 'CityID'
                //               ,CASE   CLC.ISPERSCOMP WHEN 1 THEN (CASE WHEN CLC.CODE  LIKE '195%'  THEN '1111111111' ELSE CLC.TCKNO END) ELSE  (CASE WHEN CLC.CODE  LIKE '195%'  THEN '1111111111' ELSE CLC.TAXNR END)  END  AS           'TaxNumber'
                //               ,CASE WHEN CLC.CODE  LIKE '195%'  THEN 'YOK' ELSE CLC.TAXOFFICE END AS 'TaxOffice'
                //               --,ISNULL((SELECT CODE FROM LG_XXX_EMUHACC WHERE LOGICALREF = (SELECT TOP 1 ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TYP = 1 AND TRCODE = 5 AND CARDREF = CLC.LOGICALREF)),'') AS        'AccountingCode'
                //               FROM LG_XXX_CLCARD CLC WITH(NOLOCK)
                //               WHERE CLC.ACTIVE = 0 
                //               AND CLC.CARDTYPE NOT IN(4,22) 
                //               AND CLC.EDINO = ''
                //               AND (CLC.CODE LIKE '195%')
                //               UNION ALL
                //SELECT CLC.LOGICALREF AS 'Id', CLC.CODE AS 'ClientCode', CLC.DEFINITION_ AS 'FirmName',CLC.EDINO AS 'PaymentExpCode', CLC.ADDR1 +' '+ CLC.ADDR2 AS 'Address', CLC.TOWN as 'County',  999  AS 'CityID',CASE CLC.ISPERSCOMP WHEN 1 THEN CLC.TCKNO ELSE  '1111111111' END AS 'TaxNumber','YOK' AS 'TaxOffice'
                //               --,ISNULL((SELECT CODE FROM LG_XXX_EMUHACC WHERE LOGICALREF = (SELECT TOP 1 ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TYP = 1 AND TRCODE = 5 AND CARDREF = CLC.LOGICALREF)),'') AS    'AccountingCode'
                //               FROM LG_XXX_CLCARD CLC WITH(NOLOCK)
                //               WHERE CLC.ACTIVE = 0 
                //               AND CLC.CARDTYPE NOT IN(4,22) 
                //               --AND CLC.BANKIBANS1 <> '' 
                //               AND CLC.EDINO = ''                                       
                //               AND (CLC.CODE LIKE '120%')
                //               UNION ALL
                //SELECT CLC.LOGICALREF AS 'Id', CLC.CODE AS 'ClientCode', CLC.DEFINITION_ AS 'FirmName',CLC.EDINO AS 'PaymentExpCode', CLC.ADDR1 +' '+ CLC.ADDR2 AS 'Address', CLC.TOWN as 'County',  999  AS 'CityID',CASE CLC.ISPERSCOMP WHEN 1 THEN CLC.TCKNO ELSE  '1111111111' END AS 'TaxNumber','YOK' AS 'TaxOffice'
                //              -- ,ISNULL((SELECT CODE FROM LG_XXX_EMUHACC WHERE LOGICALREF = (SELECT TOP 1 ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TYP = 1 AND TRCODE = 5 AND CARDREF = CLC.LOGICALREF)),'') AS    'AccountingCode'
                //               FROM LG_XXX_CLCARD CLC WITH(NOLOCK)
                //               WHERE CLC.ACTIVE = 0 
                //               AND CLC.CARDTYPE NOT IN(4,22) 
                //               --AND CLC.BANKIBANS1 <> '' 
                //               AND CLC.EDINO = ''                                       
                //               AND (CLC.CODE LIKE '335%')
                //                                     UNION ALL
                //SELECT CLC.LOGICALREF AS 'Id', CLC.CODE AS 'ClientCode', CLC.DEFINITION_ AS 'FirmName',CLC.EDINO AS 'PaymentExpCode', CLC.ADDR1 +' '+ CLC.ADDR2 AS 'Address', CLC.TOWN as 'County',  999  AS 'CityID',CASE CLC.ISPERSCOMP WHEN 1 THEN CLC.TCKNO ELSE  '1111111111' END AS 'TaxNumber','YOK' AS 'TaxOffice'
                //               --,ISNULL((SELECT CODE FROM LG_XXX_EMUHACC WHERE LOGICALREF = (SELECT TOP 1 ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TYP = 1 AND TRCODE = 5 AND CARDREF = CLC.LOGICALREF)),'') AS    'AccountingCode'
                //               FROM LG_XXX_CLCARD CLC WITH(NOLOCK)
                //               WHERE CLC.ACTIVE = 0 
                //               AND CLC.CARDTYPE NOT IN(4,22) 
                //               --AND CLC.BANKIBANS1 <> '' 
                //               AND CLC.EDINO = ''                                       
                //               AND (CLC.CODE LIKE '320%')".ToReplaceLogoTableName();
                var sql = string.Concat(SqlQuery.Query.FirstOrDefault(x => x.Type == "CLCARD")?.Sql).ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<ClientCard>(sql).ToList();

            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        /// <summary>
        /// Cari kart edino alanının günceller. Bulut tahsilattan gelen PN numarası set edilir.
        /// </summary>
        /// <param name="ediNo"></param>
        /// <param name="lref"></param>
        /// <returns></returns>
        public static bool UpdateClientEdiNo(string ediNo, int lref)
        {
            var flag = false;
            try
            {
                var sql = @"UPDATE LG_XXX_CLCARD SET EDINO = @EDINO WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                var parms = new[] { new SqlParameter("@EDINO", ediNo), new SqlParameter("@LREF", lref) };
                var result = dataAccess.ExecuteNonQueryWithParams(sql, parms, ref exception);
                if (result.ToInt() > 0)
                {
                    flag = true;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }

            return flag;
        }
        public static string GetClientCodeByPNCode(string pnCode)
        {
            var result = string.Empty;
            try
            {
                var sql = @"SELECT CODE FROM LG_XXX_CLCARD WHERE EDINO = @PNCODE".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<string>(sql, new { PNCODE = pnCode }).FirstOrDefault();
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static string GetClientCodeByTaxNr(string pnCode)
        {
            var result = string.Empty;
            try
            {
                var sql = @"SELECT CLC.CODE FROM LG_XXX_CLCARD WHERE TAXNR = @PNCODE".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<string>(sql, new { PNCODE = pnCode }).FirstOrDefault();
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static List<BankAccountDto> GetClientIbansByTaxNumber(string taxnr)
        {
            var result = new List<BankAccountDto>();
            try
            {
                var sql = $@"SELECT RIGHT(LEFT(BANKINFO.BRANCH,4),3) AS 'BankAccountCode',BANKINFO.BANKIBAN AS 'Iban' FROM [dbo].[fn_getBankInfos] ('{taxnr}') BANKINFO WHERE BANKINFO.BANKIBAN <> '' AND BANKINFO.BRANCH <> '' ";
                result = dataAccess.Connection.Query<BankAccountDto>(sql).ToList();
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static List<ClientCard> GetServiceCards()
        {
            var result = new List<ClientCard>();
            try
            {
                //          var sql =
                //              @"SELECT SRV.LOGICALREF, SRV.CODE,SRV.DEFINITION_,DEFINITION2,SRV.CARDTYPE FROM LG_XXX_XX_BNFLINE B
                //                  LEFT JOIN LG_XXX_XX_STLINE S ON B.SOURCEFREF = S.INVOICEREF 
                //                  LEFT JOIN LG_XXX_SRVCARD SRV ON S.STOCKREF = SRV.LOGICALREF
                //                  WHERE S.CANCELLED = 0 AND SRV.ACTIVE = 0 AND S.LINETYPE = 4 AND B.MODULENR = 7  
                //AND (DEFINITION2 = '' OR DEFINITION2 = NULL)
                //                  GROUP BY SRV.LOGICALREF,SRV.CODE,SRV.DEFINITION_,DEFINITION2,SRV.CARDTYPE".ToReplaceLogoTableName();
                //var sql = @"SELECT SRV.LOGICALREF, SRV.CODE,SRV.DEFINITION_,DEFINITION2,SRV.CARDTYPE FROM  LG_XXX_SRVCARD SRV
                //        WHERE  SRV.ACTIVE = 0
                //        AND (DEFINITION2 = '' OR DEFINITION2 = NULL) AND SPECODE4 = 'BLT'
                //        GROUP BY SRV.LOGICALREF,SRV.CODE,SRV.DEFINITION_,DEFINITION2,SRV.CARDTYPE".ToReplaceLogoTableName();
                var sql = string.Concat(SqlQuery.Query.FirstOrDefault(x => x.Type == "SRVCARD")?.Sql).ToReplaceLogoTableName();
                result = dataAccess.GetDataTable(sql, ref exception).AsEnumerable().Select(s => new ClientCard
                {
                    CityID = "999",
                    TaxOffice = "YOK",
                    TaxNumber = "1111111111",
                    FirmName = s.Field<string>("DEFINITION_"),
                    AccountingCode = s.Field<string>("CODE"),
                    ClientCode = s.Field<string>("CODE"),
                    Id = s.Field<int>("LOGICALREF")
                }).ToList();


            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        /// <summary>
        /// Hizmet kartı açıklama 2 alanının günceller. Bulut tahsilattan gelen PN numarası set edilir.
        /// </summary>
        /// <param name="ediNo"></param>
        /// <param name="lref"></param>
        /// <returns></returns>
        public static bool UpdateSrvCardDef2(string ediNo, int lref)
        {
            var flag = false;
            try
            {
                var sql = @"UPDATE LG_XXX_SRVCARD SET DEFINITION2 = @DEFINITION2 WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                var parms = new[] { new SqlParameter("@DEFINITION2", ediNo), new SqlParameter("@LREF", lref) };
                var result = dataAccess.ExecuteNonQueryWithParams(sql, parms, ref exception);
                if (result.ToInt() > 0)
                {
                    flag = true;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }

            return flag;
        }
        #endregion

        public static BankAccountDto GetBankAccountInfoByIbanNo(string ibanNo)
        {
            var result = new BankAccountDto();
            try
            {
                var sql = @"SELECT LOGICALREF AS 'Id', CARDTYPE AS 'BnCardType',BANKREF AS 'BankRef' ,CODE AS 'BankAccountCode',IBAN AS 'Iban',CURRENCY AS 'CurrencyType',CASE CURRENCY WHEN 0 THEN 'TL' WHEN 160 THEN 'TL' WHEN 1 THEN 'USD' WHEN 20 THEN 'EUR' 
WHEN 17 THEN 'GBP' ELSE '0' END AS 'CurrencyCode' FROM LG_XXX_BANKACC WHERE CODE NOT LIKE '30%' AND IBAN = @IBAN".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<BankAccountDto>(sql, new { @IBAN = ibanNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new BankAccountDto();
        }
        public static BankAccountDto GetBankAccountInfoByBankAccountCode(string bankAccountCode)
        {
            var result = new BankAccountDto();
            try
            {
                var sql = @"SELECT LOGICALREF AS 'Id', CARDTYPE AS 'BnCardType',BANKREF AS 'BankRef' ,CODE AS 'BankAccountCode',IBAN AS 'Iban',CURRENCY AS 'CurrencyType',CASE CURRENCY WHEN 0 THEN 'TL' WHEN 160 THEN 'TL' WHEN 1 THEN 'USD' WHEN 20 THEN 'EUR' 
WHEN 17 THEN 'GBP' ELSE '0' END AS 'CurrencyCode' FROM LG_XXX_BANKACC WHERE CODE = @CODE".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<BankAccountDto>(sql, new { CODE = bankAccountCode }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new BankAccountDto();
        }
        public static BankAccountDto GetBankAccountInfoByIbanNoForIncommingTransfer(string ibanNo)
        {
            var result = new BankAccountDto();
            try
            {
                var sql = @"SELECT LOGICALREF AS 'Id', CARDTYPE AS 'BnCardType',BANKREF AS 'BankRef' ,CODE AS 'BankAccountCode',IBAN AS 'Iban',CURRENCY AS 'CurrencyType',CASE CURRENCY WHEN 0 THEN 'TL' WHEN 160 THEN 'TL' WHEN 1 THEN 'USD' WHEN 20 THEN 'EUR' 
WHEN 17 THEN 'GBP' ELSE '0' END AS 'CurrencyCode' FROM LG_XXX_BANKACC WHERE IBAN = @IBAN".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<BankAccountDto>(sql, new { @IBAN = ibanNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new BankAccountDto();
        }
        public static BankAccountDto GetBankAccountInfoByIbanNoForSendingTransfer(string ibanNo)
        {
            var result = new BankAccountDto();
            try
            {
                var sql = @"SELECT LOGICALREF AS 'Id', CARDTYPE AS 'BnCardType',BANKREF AS 'BankRef' ,CODE AS 'BankAccountCode',IBAN AS 'Iban',CURRENCY AS 'CurrencyType',CASE CURRENCY WHEN 0 THEN 'TL' WHEN 160 THEN 'TL' WHEN 1 THEN 'USD' WHEN 20 THEN 'EUR' 
WHEN 17 THEN 'GBP' ELSE '0' END AS 'CurrencyCode' FROM LG_XXX_BANKACC WHERE IBAN = @IBAN".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<BankAccountDto>(sql, new { @IBAN = ibanNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new BankAccountDto();
        }
        public static string GetGlobalBankCodeByIbanNo(string ibanNo)
        {
            var result = string.Empty;
            try
            {
                var sql = "SELECT RIGHT(LEFT(BRANCHNO,4),3) AS 'BRANCHNO' FROM LG_XXX_BNCARD WHERE LOGICALREF = (SELECT BANKREF FROM LG_XXX_BANKACC WHERE IBAN = @IBAN)".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<string>(sql, new { @IBAN = ibanNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static ClientPairInfoDto GetClientPairInfoByPaymetExpCode(string ibanNo)
        {
            var result = new ClientPairInfoDto();
            try
            {
                var sql = "SELECT BANKIBANS1 AS 'IBAN',CASE WHEN ISPERSCOMP = 0 THEN TAXNR WHEN ISPERSCOMP = 1 THEN TCKNO END AS 'TAXNR',ISNULL(NAME,'') AS 'NAME',ISNULL(SURNAME,'') AS 'SURNAME' FROM LG_XXX_CLCARD WHERE EDINO = @EDINO".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<ClientPairInfoDto>(sql, new { EDINO = ibanNo }).FirstOrDefault();
            }
            catch (Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static ServicePairInfoDto GetServiceCardPairInfoByPaymetExpCode(string ibanNo)
        {
            var result = new ServicePairInfoDto();
            try
            {
                var sql = "SELECT CARDTYPE AS 'CardType',CODE AS 'SrvCode',DEFINITION2 AS 'PnCode' FROM LG_XXX_SRVCARD WHERE DEFINITION2 = @DEFINITION2".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<ServicePairInfoDto>(sql, new { DEFINITION2 = ibanNo }).FirstOrDefault();
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static string GetBnFicheNumberByRef(int lref, ref DataAccessException exception)
        {
            var result = string.Empty;
            string sql = @"SELECT FICHENO FROM LG_XXX_XX_BNFICHE WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
            var parms = new[]
            {
                new SqlParameter("@LREF", lref)
            };
            result = dataAccess.GetDataTableByParams(sql, parms, ref exception).AsEnumerable().Select(s => s.Field<string>("FICHENO")).FirstOrDefault();

            return result;
        }
        public static string GetCheckFicheNumberByRef(int lref, ref DataAccessException exception)
        {
            var result = string.Empty;
            string sql = @"SELECT ROLLNO FROM LG_XXX_XX_CSROLL WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
            var parms = new[]
            {
                new SqlParameter("@LREF", lref)
            };
            result = dataAccess.GetDataTableByParams(sql, parms, ref exception).AsEnumerable().Select(s => s.Field<string>("ROLLNO")).FirstOrDefault();

            return result;
        }
        public static ExchangeDto GetDailyExchange(int currType, DateTime currDate)
        {
            var firm = GlobalSettings.Firm.FirmNumber;
            var query = "SELECT SEPEXCHTABLE FROM L_CAPIFIRM WHERE NR = @NR";
            var parms = new[]
            {
                new SqlParameter("@NR", firm)
            };
            var result = new ExchangeDto();
            var flag = dataAccess.GetDataTableByParams(query, parms, ref exception).AsEnumerable().Select(s => s.Field<short>("SEPEXCHTABLE")).FirstOrDefault();

            string sql = string.Concat(@"SELECT LREF, dbo.fn_LogoDatetoSystemDate(DATE_)DATE_,RATES1,RATES2,RATES3,RATES4,EDATE FROM ", flag == 0 ? " L_DAILYEXCHANGES" : flag == 1 ? "LG_EXCHANGE_XXX" : "L_DAILYEXCHANGES", $" WITH(NOLOCK) WHERE CRTYPE IN ({currType}) AND (DATE_ = DAY('{currDate:yyyyMMdd}') + 256 * MONTH('{currDate:yyyyMMdd}') + 65536 * YEAR('{currDate:yyyyMMdd}'))").ToReplaceLogoTableName();

            result = dataAccess.GetDataTable(sql, ref exception).AsEnumerable().Select(s => new ExchangeDto
            {
                LREF = s.Field<int>("LREF"),
                DATE_ = s.Field<DateTime>("DATE_"),
                RATES1 = s.Field<double>("RATES1"),
                RATES2 = s.Field<double>("RATES2"),
                RATES3 = s.Field<double>("RATES3"),
                RATES4 = s.Field<double>("RATES4"),
                EDATE = s.Field<DateTime>("EDATE")
            }).FirstOrDefault();
            if (result == null)
            {
                result = new ExchangeDto
                {
                    RATES1 = 1,
                    RATES2 = 1,
                    RATES3 = 1,
                    RATES4 = 1,
                };
            }

            return result;
        }
        public static int GetEmuAccRefBankAccountCode(string bankAccountCode)
        {
            var result = 0;
            try
            {
                var sql = @"SELECT TOP 1 ACCOUNTREF FROM LG_XXX_ACCCODES WHERE MODNR = 45 AND INDEXCODE = @INDEXCODE".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { @INDEXCODE = bankAccountCode }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static int GetEmuAccRefBankAccountRef(int bankAccountRef)
        {
            var result = 0;
            try
            {
                var sql = @"SELECT ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TRCODE = 6 AND TYP = 4 AND CARDREF = @CARDREF  ".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { @CARDREF = bankAccountRef }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static string GetEmuAccRefBankAccount(int cardRef, int trCode, int type)
        {
            var result = string.Empty;
            try
            {
                var sql = @"SELECT CODE FROM LG_XXX_EMUHACC WHERE LOGICALREF = (SELECT ACCOUNTREF FROM LG_XXX_CRDACREF WHERE TRCODE = @TRCODE AND TYP = @TYP AND CARDREF = @CARDREF ) ".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<string>(sql, new { CARDREF = cardRef, TRCODE = trCode, TYP = type }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static string GetEmFicheNumberByRef(int lref, ref DataAccessException exception)
        {
            var result = string.Empty;
            string sql = @"SELECT FICHENO FROM LG_XXX_XX_EMFICHE WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
            var parms = new[]
            {
                new SqlParameter("@LREF", lref)
            };
            result = dataAccess.GetDataTableByParams(sql, parms, ref exception).AsEnumerable().Select(s => s.Field<string>("FICHENO")).FirstOrDefault();

            return result;
        }
        public static ChequeDto GetCheckRefByCheckNo(string checkNumber, int proctype)
        {
            var result = new ChequeDto();
            try
            {
                var sql = @"SELECT LOGICALREF AS 'CheckRef',PORTFOYNO AS 'PortfoyNo',NEWSERINO AS 'NewSeriNo',DOC AS 'ProcType',CURRSTAT AS 'CurrStat' FROM LG_XXX_XX_CSCARD WHERE (PORTFOYNO = @PORTFOYNO OR NEWSERINO = @NEWSERINO) AND DOC = @DOC".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<ChequeDto>(sql, new { PORTFOYNO = checkNumber, NEWSERINO = checkNumber, DOC = proctype }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new ChequeDto();
        }
        public static int GetCheckClientRefByCheckRef(int checkRef)
        {
            var result = 0;
            try
            {
                var sql = @"SELECT CST.CARDREF FROM LG_XXX_XX_CSTRANS  CST
LEFT JOIN LG_XXX_XX_CSROLL CSR ON CST.ROLLREF = CSR.LOGICALREF  WHERE CST.TRCODE = 3 AND CST.STATUS = 9  AND CST.CARDMD = 5  AND CSREF = @CSREF".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { CSREF = checkRef }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static int GetBnCardTypeByBankAccountCode(string bankAccountCode)
        {
            var result = 0;
            try
            {
                var sql = @"SELECT CARDTYPE FROM LG_XXX_BANKACC WHERE CODE = @CODE".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { CODE = bankAccountCode }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }

        #region Leasing İşlemleri
        public static int GetLeasingRefByRegNumber(string leasingRegNo)
        {
            var result = 0;
            try
            {
                var sql = @"SELECT LOGICALREF FROM LG_XXX_LEASINGREG WHERE REGNR = @REGNR".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { REGNR = leasingRegNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static LeasingPaymet GetLeasingPaymetInfo(int leasingRef, DateTime paymentDate)
        {
            var result = new LeasingPaymet();
            try
            {
                var sql = @"SELECT C.CODE,L.LOGICALREF ,L.FICHEREF ,L.LINENR ,L.PAYMENTDATE ,L.PAYMENTTOTAL ,L.INTTOTAL ,L.MAINTOTAL ,L.MAINREMAINED ,L.TRCURR ,L.TRRATE ,L.BOSTATUS ,L.VATINPAYMENTTOTAL 
                                ,L.PAYMENTTYPE ,L.SITEID ,L.RECSTATUS ,L.ORGLOGICREF ,L.WFSTATUS ,L.CAPIBLOCK_CREATEDBY ,L.CAPIBLOCK_CREADEDDATE ,L.CAPIBLOCK_CREATEDHOUR ,L.CAPIBLOCK_CREATEDMIN 
                                ,L.CAPIBLOCK_CREATEDSEC ,L.CAPIBLOCK_MODIFIEDBY ,L.CAPIBLOCK_MODIFIEDDATE ,L.CAPIBLOCK_MODIFIEDHOUR ,L.CAPIBLOCK_MODIFIEDMIN ,L.CAPIBLOCK_MODIFIEDSEC ,L.BNACCREF 
                                ,L.LEASINGREF ,L.TRANSTYPE ,L.PARENTREF --,L.BNFCHREF ,L.DEPRECIATION ,L.PROFITLOSS ,L.ACTACCREF ,L.RENTOBL ,L.INTRATE ,L.NPVTODAYVAL ,L.TENANCY 
                                FROM LG_XXX_LEASINGPAYMENTSLNS L
                                JOIN LG_XXX_PURCHOFFER P ON P.LOGICALREF = L.FICHEREF
                                JOIN LG_XXX_CLCARD C ON C.LOGICALREF = P.CLIENTREF
                                WHERE L.LEASINGREF = @LEASINGREF AND L.TRANSTYPE = 0 
                                AND DATEPART(DAY,L.PAYMENTDATE) = DATEPART(DAY,@PAYMENTDATE) 
                                AND DATEPART(MONTH,L.PAYMENTDATE) = DATEPART(MONTH,@PAYMENTDATE) 
                                AND DATEPART(YEAR,L.PAYMENTDATE) = DATEPART(YEAR,@PAYMENTDATE)".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<LeasingPaymet>(sql, new { LEASINGREF = leasingRef, PAYMENTDATE = paymentDate }).FirstOrDefault();
            }
            catch (System.Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new LeasingPaymet();
        }

        public static ServiceResult InsertLeasingPayment(LeasingPaymet leasingPaymet)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var sql =
                    @"INSERT INTO LG_XXX_LEASINGPAYMENTSLNS (FICHEREF ,LINENR ,PAYMENTDATE ,PAYMENTTOTAL ,INTTOTAL ,MAINTOTAL ,MAINREMAINED ,TRCURR ,TRRATE ,BOSTATUS ,VATINPAYMENTTOTAL ,PAYMENTTYPE ,SITEID ,RECSTATUS ,ORGLOGICREF ,WFSTATUS ,CAPIBLOCK_CREATEDBY ,CAPIBLOCK_CREADEDDATE ,CAPIBLOCK_CREATEDHOUR ,CAPIBLOCK_CREATEDMIN ,CAPIBLOCK_CREATEDSEC ,CAPIBLOCK_MODIFIEDBY ,CAPIBLOCK_MODIFIEDHOUR ,CAPIBLOCK_MODIFIEDMIN ,CAPIBLOCK_MODIFIEDSEC ,BNACCREF ,LEASINGREF ,TRANSTYPE ,PARENTREF ,BNFCHREF) 
                      VALUES (@FICHEREF ,@LINENR ,@PAYMENTDATE ,@PAYMENTTOTAL ,@INTTOTAL ,@MAINTOTAL ,@MAINREMAINED ,@TRCURR ,@TRRATE ,@BOSTATUS ,@VATINPAYMENTTOTAL ,@PAYMENTTYPE ,@SITEID ,@RECSTATUS 
                      ,@ORGLOGICREF ,@WFSTATUS ,@CAPIBLOCK_CREATEDBY ,@CAPIBLOCK_CREADEDDATE ,@CAPIBLOCK_CREATEDHOUR ,@CAPIBLOCK_CREATEDMIN ,@CAPIBLOCK_CREATEDSEC ,@CAPIBLOCK_MODIFIEDBY ,@CAPIBLOCK_MODIFIEDHOUR ,@CAPIBLOCK_MODIFIEDMIN ,@CAPIBLOCK_MODIFIEDSEC ,@BNACCREF ,@LEASINGREF ,@TRANSTYPE ,@PARENTREF ,@BNFCHREF)".ToReplaceLogoTableName();
                serviceResult.RowCount = dataAccess.Connection.Execute(sql, leasingPaymet);
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }

            return serviceResult;
        }

        public static int UpdateBnFicheLeasingPayment(int leasingRef, int ficheRef)
        {
            var result = 0;
            try
            {
                var sql = $@"UPDATE LG_XXX_XX_BNFICHE SET BNCRREF = {leasingRef} WHERE  LOGICALREF = {ficheRef}".ToReplaceLogoTableName();
                result = dataAccess.Connection.Execute(sql);
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }

        #endregion

        #region Kredi İşlemleri
        public static int GetBankCreditRefByCreditNumber(string creditNo)
        {
            var result = 0;
            try
            {
                var sql = @"SELECT LOGICALREF FROM LG_XXX_BNCREDITCARD WHERE CODE = @CODE".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<int>(sql, new { CODE = creditNo }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static CreditPayment GetBankCreditPaymetInfo(int creditRef, DateTime paymentDate)
        {
            var result = new CreditPayment();
            try
            {
                var sql = @"SELECT TOP 1 CRD.CODE, CRD.BNCRACCREF,CRD.BNCRREF,PAY.LOGICALREF ,PAY.CREDITREF ,PAY.PERNR ,PAY.TRANSTYPE ,PAY.PARENTREF ,PAY.DUEDATE ,PAY.OPRDATE ,PAY.LINENR ,PAY.TOTAL ,PAY.INTTOTAL ,PAY.BSMVTOTAL ,PAY.KKDFTOTAL ,PAY.BNFCHREF ,PAY.MODIFIED ,PAY.BNACCREF ,PAY.TRRATECR ,PAY.TRRATEACC ,PAY.EARLYINTRATE ,PAY.EARLYINTTOT ,PAY.LATEINTRATE ,PAY.LATEINTTOT ,PAY.CAPIBLOCK_CREATEDBY ,PAY.CAPIBLOCK_CREADEDDATE ,PAY.CAPIBLOCK_CREATEDHOUR ,PAY.CAPIBLOCK_CREATEDMIN ,PAY.CAPIBLOCK_CREATEDSEC ,PAY.CAPIBLOCK_MODIFIEDBY ,PAY.CAPIBLOCK_MODIFIEDDATE ,PAY.CAPIBLOCK_MODIFIEDHOUR ,PAY.CAPIBLOCK_MODIFIEDMIN ,PAY.CAPIBLOCK_MODIFIEDSEC ,PAY.SITEID ,PAY.ORGLOGICALREF ,PAY.RECSTATUS ,PAY.WFSTATUS ,PAY.LINEEXP ,PAY.INTRATE ,PAY.BSMVRATE ,PAY.FROMCREDITCLOSE ,PAY.TRANSPECODE ,PAY.TRANLINEEXP ,PAY.STRUCTED ,PAY.SBSMVRATE ,PAY.SINTRATE ,PAY.BNCRPARENTREF FROM LG_XXX_BNCREPAYTR PAY LEFT JOIN LG_XXX_BNCREDITCARD CRD ON CRD.LOGICALREF = PAY.CREDITREF WHERE TRANSTYPE = 0 AND  PAY.CREDITREF = @CREDITREF AND PAY.LOGICALREF NOT IN (SELECT PARENTREF FROM LG_XXX_BNCREPAYTR WHERE CREDITREF = @CREDITREF AND TRANSTYPE = 1) ORDER BY LOGICALREF".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<CreditPayment>(sql, new { CREDITREF = creditRef }).FirstOrDefault();
            }
            catch (System.Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? new CreditPayment();
        }

        public static string GetCreditBankAccountInfo(int bankAccountRef)
        {
            var result = string.Empty;
            try
            {
                var sql = @"SELECT CODE FROM LG_XXX_BANKACC WHERE LOGICALREF = @LOGICALREF".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<string>(sql, new { LOGICALREF = bankAccountRef }).FirstOrDefault();
            }
            catch (System.Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result ?? string.Empty;
        }

        public static ServiceResult InsertBankCreditPayment(CreditPayment creditPayment)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var sql =
                    @"INSERT INTO LG_XXX_BNCREPAYTR (CREDITREF ,PERNR ,TRANSTYPE ,PARENTREF ,DUEDATE ,OPRDATE ,LINENR ,TOTAL ,INTTOTAL ,BSMVTOTAL ,KKDFTOTAL ,BNFCHREF ,MODIFIED ,BNACCREF ,TRRATECR ,TRRATEACC ,EARLYINTRATE ,EARLYINTTOT ,LATEINTRATE ,LATEINTTOT ,CAPIBLOCK_CREATEDBY ,CAPIBLOCK_CREADEDDATE ,CAPIBLOCK_CREATEDHOUR ,CAPIBLOCK_CREATEDMIN ,CAPIBLOCK_CREATEDSEC ,CAPIBLOCK_MODIFIEDBY ,CAPIBLOCK_MODIFIEDDATE ,CAPIBLOCK_MODIFIEDHOUR ,CAPIBLOCK_MODIFIEDMIN ,CAPIBLOCK_MODIFIEDSEC ,SITEID ,ORGLOGICALREF ,RECSTATUS ,WFSTATUS ,LINEEXP ,INTRATE ,BSMVRATE ,FROMCREDITCLOSE ,TRANSPECODE ,TRANLINEEXP ,STRUCTED ,SBSMVRATE ,SINTRATE ,BNCRPARENTREF) VALUES (@CREDITREF ,@PERNR ,@TRANSTYPE ,@PARENTREF ,@DUEDATE ,@OPRDATE ,@LINENR ,@TOTAL ,@INTTOTAL ,@BSMVTOTAL ,@KKDFTOTAL ,@BNFCHREF ,@MODIFIED ,@BNACCREF ,@TRRATECR ,@TRRATEACC ,@EARLYINTRATE ,@EARLYINTTOT ,@LATEINTRATE ,@LATEINTTOT ,@CAPIBLOCK_CREATEDBY ,@CAPIBLOCK_CREADEDDATE ,@CAPIBLOCK_CREATEDHOUR ,@CAPIBLOCK_CREATEDMIN ,@CAPIBLOCK_CREATEDSEC ,@CAPIBLOCK_MODIFIEDBY ,@CAPIBLOCK_MODIFIEDDATE ,@CAPIBLOCK_MODIFIEDHOUR ,@CAPIBLOCK_MODIFIEDMIN ,@CAPIBLOCK_MODIFIEDSEC ,@SITEID ,@ORGLOGICALREF ,@RECSTATUS ,@WFSTATUS ,@LINEEXP ,@INTRATE ,@BSMVRATE ,@FROMCREDITCLOSE ,@TRANSPECODE ,@TRANLINEEXP ,@STRUCTED ,@SBSMVRATE ,@SINTRATE ,@BNCRPARENTREF)".ToReplaceLogoTableName();
                serviceResult.RowCount = dataAccess.Connection.Execute(sql, creditPayment);
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }

            return serviceResult;
        }

        public static int UpdateBnFicheCreditPayment(int leasingRef, int ficheRef)
        {
            var result = 0;
            try
            {
                var sql = $@"UPDATE LG_XXX_XX_BNFICHE SET LEASINGREF = {leasingRef} WHERE  LOGICALREF = {ficheRef}".ToReplaceLogoTableName();
                result = dataAccess.Connection.Execute(sql);
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }

        #endregion

        #region Çek Virmanı İşlemleri
        public static BankAccountDto GetCrossBankTransferAccountCode(string ibanNo, int currency)
        {
            var result = new BankAccountDto();
            try
            {
                var sql = @"SELECT LOGICALREF AS 'Id', CARDTYPE AS 'BnCardType',BANKREF AS 'BankRef' ,CODE AS 'BankAccountCode',IBAN AS 'Iban',CURRENCY AS 'CurrencyType',CASE CURRENCY WHEN 0 THEN 'TL' WHEN 160 THEN 'TL' WHEN 1 THEN 'USD' WHEN 20 THEN 'EUR' 
WHEN 17 THEN 'GBP' ELSE '0' END AS 'CurrencyCode' FROM LG_XXX_BANKACC WHERE BANKREF = (SELECT TOP 1 LOGICALREF FROM LG_XXX_BNCARD WHERE CODE LIKE '30.%' 
                AND RIGHT(LEFT(BRANCHNO,4),3) = (SELECT TOP 1 RIGHT(LEFT(BRANCHNO, 4), 3) AS 'BRANCHNO' FROM LG_XXX_BNCARD
                WHERE LOGICALREF = (SELECT TOP 1 BANKREF FROM LG_XXX_BANKACC WHERE IBAN = @IBAN)))
                AND CARDTYPE NOT IN(5,6) AND CURRENCY = @CURRENCY
                ".ToReplaceLogoTableName();
                result = dataAccess.Connection.Query<BankAccountDto>(sql, new { IBAN = ibanNo, CURRENCY = currency }).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }

        #endregion

        #region Log İşlemleri
        public static ServiceResult InsertGetLog(GetLog getLog, ref DataAccessException exception)
        {
            var flag = new ServiceResult();
            try
            {
                var query = @"INSERT INTO BTI_GETLOG(PROCDATE,OPERATIONNAME,PAYMENTSTATUSTYPEID,BEGDATE,ENDDATE,METHODNAME,FILTERS,RAWDATA,FILTEREDDATA)
                VALUES(@PROCDATE,@OPERATIONNAME, @PAYMENTSTATUSTYPEID, @BEGDATE, @ENDDATE, @METHODNAME, @FILTERS, @RAWDATA, @FILTEREDDATA)";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                    new SqlParameter("@PROCDATE",getLog.ProcDate),
                    new SqlParameter("@OPERATIONNAME",getLog.OperationName),
                    new SqlParameter("@PAYMENTSTATUSTYPEID", getLog.PaymentStatusTypeId),
                    new SqlParameter("@BEGDATE",getLog.BegDate),
                    new SqlParameter("@ENDDATE",getLog.EndDate),
                    new SqlParameter("@METHODNAME",getLog.MethodName),
                    new SqlParameter("@FILTERS",getLog.Filters),
                    new SqlParameter("@RAWDATA",getLog.RawData),
                    new SqlParameter("@FILTEREDDATA",getLog.FilteredData)
                };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }
        public static ServiceResult InsertPostLog(PostLog postLog, ref DataAccessException exception)
        {
            var flag = new ServiceResult();
            try
            {
                var query = "INSERT INTO BTI_POSTLOG(HOSTIP,POSTDATE,IDENTITYNAME,OPERATIONTYPE,URL,REQUESTMETHOD,JSONDATA) VALUES (@HOSTIP,@POSTDATE,@IDENTITYNAME,@OPERATIONTYPE,@URL,@REQUESTMETHOD,@JSONDATA)";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                //new SqlParameter("@ID",postLog.Id),
                new SqlParameter("@HOSTIP", postLog.HostIp),
                new SqlParameter("@POSTDATE",postLog.PostDate),
                new SqlParameter("@IDENTITYNAME",postLog.IdentityName),
                new SqlParameter("@OPERATIONTYPE",postLog.OperationType),
                new SqlParameter("@URL",postLog.Url),
                new SqlParameter("@REQUESTMETHOD",postLog.RequestMethod),
                new SqlParameter("@JSONDATA",postLog.JsonData)
            };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }
        public static string GetInvoiceNumberByRef(int lref)
        {
            var result = string.Empty;
            try
            {
                string sql = @"SELECT FICHENO FROM LG_XXX_XX_INVOICE WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                //result = _dataAccess.GetDataTableByParams(sql, parms, ref exception).AsEnumerable().Select(s => s.Field<string>("FICHENO")).FirstOrDefault();
                result = dataAccess.Connection.Query<string>(sql, new { LREF = lref }).FirstOrDefault(); //.ExecuteScalar(sql, parms, ref exception).ToString();

            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }

            return result;
        }
        public static ServiceResult InsertResponseLog(ResponseLog responseLog, ref DataAccessException exception)
        {
            var flag = new ServiceResult();
            try
            {
                var query = "INSERT INTO BTI_RESPONSELOG(POSTREF,HOSTIP,POSTDATE,IDENTITYNAME,OPERATIONTYPE,RESPONSESTATUS,JSONDATA,RESPONSEDATA) VALUES (@POSTREF,@HOSTIP,@POSTDATE,@IDENTITYNAME,@OPERATIONTYPE,@RESPONSESTATUS,@JSONDATA,@RESPONSEDATA)";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                new SqlParameter("@POSTREF",responseLog.PostId),
                new SqlParameter("@HOSTIP", responseLog.HostIp),
                new SqlParameter("@POSTDATE",responseLog.PostDate),
                new SqlParameter("@IDENTITYNAME",responseLog.IdentityName),
                new SqlParameter("@OPERATIONTYPE",responseLog.OperationType),
                new SqlParameter("@RESPONSESTATUS",responseLog.ResponseStatus),
                new SqlParameter("@JSONDATA",responseLog.JsonData),
                new SqlParameter("@RESPONSEDATA",responseLog.ResponseData)
            };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }
        public static ServiceResult InsertErrorLog(ErrorLog errorLog, ref DataAccessException exception)
        {
            var flag = new ServiceResult();
            try
            {
                var query = "INSERT INTO BTI_ERRORLOG(POSTREF,HOSTIP,POSTDATE,IDENTITYNAME,OPERATIONTYPE,ERRORCLASSNAME,ERRORMETHODNAME,ERRORMESSAGE,INNEREXCEPTION,JSONDATA,RESPONSEDATA) VALUES (@POSTREF,@HOSTIP,@POSTDATE,@IDENTITYNAME,@OPERATIONTYPE,@ERRORCLASSNAME,@ERRORMETHODNAME,@ERRORMESSAGE,@INNEREXCEPTION,@JSONDATA,@RESPONSEDATA)";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                new SqlParameter("@POSTREF",errorLog.PostId),
                new SqlParameter("@HOSTIP", errorLog.HostIp),
                new SqlParameter("@POSTDATE",errorLog.PostDate),
                new SqlParameter("@IDENTITYNAME",errorLog.IdentityName),
                new SqlParameter("@OPERATIONTYPE",errorLog.OperationType),
                new SqlParameter("@ERRORCLASSNAME",errorLog.ErrorClassName),
                new SqlParameter("@ERRORMETHODNAME",errorLog.ErrorMethodName),
                new SqlParameter("@ERRORMESSAGE",errorLog.ErrorMessage),
                new SqlParameter("@INNEREXCEPTION",errorLog.InnerException??string.Empty),
                new SqlParameter("@JSONDATA",errorLog.JsonData),
                new SqlParameter("@RESPONSEDATA",errorLog.ResponseData)
            };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }
        public static ServiceResult InsertTransferErrorLog(TransferErrorLog errorLog, ref DataAccessException exception)
        {
            var flag = new ServiceResult();
            try
            {
                var query = @"INSERT INTO BTI_TRANSFER_ERRORLOG(PROCDATE,PAYMENTID,FICHEDATE,PAYMENTEXPCODE,BULUTFICHETYPE,LOGOFICHETYPE,SENDERFIRMNAME,SENDERBANK,SENDERIBAN,BRANCHFIRMNAME,FIRMBANK,FIRMIBAN,EXPLANATION,PAYMENTDATA,ERROR,ERPTRANSFEREXP,ERPFICHENO,ERPRESPONSE,ERPPOSTJSON,STATUSINFO)
                VALUES(@PROCDATE,@PAYMENTID, @FICHEDATE, @PAYMENTEXPCODE, @BULUTFICHETYPE, @LOGOFICHETYPE, @SENDERFIRMNAME, @SENDERBANK, @SENDERIBAN, @BRANCHFIRMNAME, @FIRMBANK, @FIRMIBAN, @EXPLANATION,@PAYMENTDATA, @ERROR,@ERPTRANSFEREXP,@ERPFICHENO,@ERPRESPONSE,@ERPPOSTJSON,@STATUSINFO)";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                    new SqlParameter("@PAYMENTID",errorLog.PAYMENTID),
                    new SqlParameter("@PROCDATE",DateTime.Now),
                    new SqlParameter("@FICHEDATE", errorLog.FICHEDATE),
                    new SqlParameter("@PAYMENTEXPCODE",errorLog.PAYMENTEXPCODE??string.Empty),
                    new SqlParameter("@BULUTFICHETYPE",errorLog.BULUTFICHETYPE??string.Empty),
                    new SqlParameter("@LOGOFICHETYPE",errorLog.LOGOFICHETYPE??string.Empty),
                    new SqlParameter("@SENDERFIRMNAME",errorLog.SENDERFIRMNAME??string.Empty),
                    new SqlParameter("@SENDERBANK",errorLog.SENDERBANK??string.Empty),
                    new SqlParameter("@SENDERIBAN",errorLog.SENDERIBAN??string.Empty),
                    new SqlParameter("@BRANCHFIRMNAME",errorLog.BRANCHFIRMNAME??string.Empty),
                    new SqlParameter("@FIRMBANK",errorLog.FIRMBANK??string.Empty),
                    new SqlParameter("@FIRMIBAN",errorLog.FIRMIBAN??string.Empty),
                    new SqlParameter("@EXPLANATION",errorLog.EXPLANATION??string.Empty),
                    new SqlParameter("@PAYMENTDATA",errorLog.PAYMENTDATA??string.Empty),
                    new SqlParameter("@ERROR",errorLog.ERROR??string.Empty),
                    new SqlParameter("@ERPTRANSFEREXP",errorLog.ERPTRANSFEREXP??string.Empty),
                    new SqlParameter("@ERPFICHENO",errorLog.ERPFICHENO??string.Empty),
                    new SqlParameter("@ERPRESPONSE",errorLog.ERPRESPONSE??string.Empty),
                    new SqlParameter("@ERPPOSTJSON",errorLog.ERPPOSTJSON??string.Empty),
                    new SqlParameter("@STATUSINFO",errorLog.STATUSINFO??string.Empty),

                };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                    LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", exception.ErrorDesc, JsonConvert.SerializeObject(errorLog, Formatting.Indented)));
                }
            }
            catch (Exception e)
            {
                LogHelper.LogDbError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }

        #endregion

        public static ServiceResult InsertBankBalance(List<BankBalances> bankBalance)
        {
            var serviceResult = new ServiceResult();
            try
            {
                serviceResult.RowCount = dataAccess.Connection.Insert(bankBalance).ToInt();


                if (serviceResult.RowCount > 0)
                {
                    serviceResult.Success = true;
                    serviceResult.ObjectNo = serviceResult.RowCount;
                }
            }
            catch (Exception e)
            {
                serviceResult.Success = false;
                serviceResult.ErrorDesc = e.Message;
                LogHelper.LogError(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    typeof(DataLogic), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name, StringUtil.Seperator, e.Message));
            }
            return serviceResult;
        }

        #region Token Opreration
        public static string GetAccessTokenFromSql(string firmNo, string userName)
        {
            var result = string.Empty;
            try
            {
                var sql = "SELECT ACCESSTOKEN FROM BTI_ERPTOKENS WITH(NOLOCK) WHERE EXPIREDATE > GETDATE() AND FIRMNR = @FIRMNR AND USERNAME = @USERNAME".ToReplaceLogoTableName();
                var parms = new[]
                {
                    new SqlParameter("@FIRMNR", firmNo),
                    new SqlParameter("@USERNAME", userName)
                };
                result = dataAccess.GetDataTableByParams(sql, parms, ref exception).AsEnumerable().Select(s => s.Field<string>("ACCESSTOKEN")).FirstOrDefault();
            }
            catch (System.Exception e)
            {

                LogHelper.LogError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }
            return result;
        }
        public static ServiceResult InsertAccessToken(ErpTokens erpTokens)
        {
            var flag = new ServiceResult();
            try
            {
                var query = @"IF NOT EXISTS(SELECT LREF FROM BTI_ERPTOKENS WHERE FIRMNR = @FIRMNR AND USERNAME = @USERNAME)
                                 BEGIN
                                     INSERT INTO BTI_ERPTOKENS(ACCESSTOKEN, EXPIREDATE, FIRMNR, USERNAME) VALUES(@ACCESSTOKEN, DATEADD(MINUTE, 115, GETDATE()), @FIRMNR, @USERNAME)
                                 END
                                     ELSE
                                 BEGIN
                                     UPDATE BTI_ERPTOKENS SET ACCESSTOKEN = @ACCESSTOKEN, EXPIREDATE = DATEADD(MINUTE, 115, GETDATE())  WHERE FIRMNR = @FIRMNR AND USERNAME = @USERNAME
                                 END";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                    new SqlParameter("@FIRMNR",erpTokens.FirmNo),
                    new SqlParameter("@USERNAME", erpTokens.UserName),
                    new SqlParameter("@ACCESSTOKEN",erpTokens.AccessToken)
                };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }
        public static ServiceResult UpdateAccessTokenExpriedDate(ErpTokens erpTokens)
        {
            var flag = new ServiceResult();
            try
            {
                var query = @"UPDATE BTI_ERPTOKENS SET EXPIREDATE =  GETDATE() WHERE FIRMNR = @FIRMNR AND USERNAME = @USERNAME";
                var sql = query + ";SELECT SCOPE_IDENTITY();";
                var parms = new[]
                {
                    new SqlParameter("@FIRMNR",erpTokens.FirmNo),
                    new SqlParameter("@USERNAME", erpTokens.UserName)
                };
                var result = dataAccess.ExecuteScalar(sql, parms, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }

        public static ServiceResult TruncateErpTokens()
        {
            var flag = new ServiceResult();
            try
            {
                var sql = @"TRUNCATE TABLE BTI_ERPTOKENS";
                var result = dataAccess.ExecuteScalar(sql, ref exception).ToInt();
                if (result > 0)
                {
                    flag.Success = true;
                    flag.LogRef = result;
                }
                else
                {
                    flag.ErrorDesc = exception.ErrorDesc;
                }
            }
            catch (Exception e)
            {
                LogHelper.LogError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }


            return flag;
        }

        #endregion
        public static string GetFicheNumberOrCardCodeByRef(int lref, string dataType)
        {
            var result = string.Empty;
            try
            {
                string sql = string.Empty;
                string columnName = string.Empty;
                string ficheNo = "FICHENO";
                string code = "CODE";
                switch (dataType)
                {
                    case "salesInvoices":
                        sql = @"SELECT FICHENO FROM LG_XXX_XX_INVOICE WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                        columnName = ficheNo;
                        break;
                    case "Arps":
                        sql = @"SELECT CODE FROM LG_XXX_CLCARD WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                        columnName = code;
                        break;
                    case "projects":
                        sql = @"SELECT CODE FROM LG_XXX_PROJECTS WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                        columnName = code;
                        break;
                    case "soldServices":
                        sql = @"SELECT CODE FROM LG_XXX_SRVCARD WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                        columnName = code;
                        break;
                    case "GLAccounts":
                        sql = @"SELECT CODE FROM LG_XXX_SRVCARD WHERE LOGICALREF = @LREF".ToReplaceLogoTableName();
                        columnName = code;
                        break;
                }

                var parms = new[]
                {
                new SqlParameter("@LREF", lref)
            };
                result = dataAccess.GetDataTableByParams(sql, parms, ref exception).AsEnumerable().Select(s => s.Field<string>(columnName)).FirstOrDefault();

            }
            catch (Exception e)
            {
                LogHelper.LogError(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", e.Message));
            }

            return result;
        }
    }
}
