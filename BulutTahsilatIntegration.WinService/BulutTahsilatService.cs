using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.DataAccess;
using BulutTahsilatIntegration.WinService.Job;
using BulutTahsilatIntegration.WinService.Model.Global;
using BulutTahsilatIntegration.WinService.Utilities;

namespace BulutTahsilatIntegration.WinService
{
    public partial class BulutTahsilatService : ServiceBase
    {
        public BulutTahsilatService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (File.Exists(ConfigHelper.ReadPath))
                {
                    var config = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath);
                    GlobalSettings.MailTo = config.MailTo;
                    GlobalSettings.Firms = config.Firms;
                }
                LogHelper.Log("<!----------------------Starting Service------------------------------!>");
                CreateTable();
                CreateFunction();
                //string message = string.Empty;
                //DataConfig.CheckAndCreateDBObjects(out message);
                //Scheduler.StartJob().GetAwaiter().GetResult();
                //var client = new JobClient();
                //client.Execute();
                Scheduler.StartJobClient().GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                string.Concat(LogHelper.LogType.Error.ToLogType(), " ", nameof(BulutTahsilatService), " ", MethodBase.GetCurrentMethod().Name, " ",
                    "BTI_TRANSFER_ERRORLOG tablosu oluşturulamadı!", exception.Message);
            }
        }

        protected override void OnStop()
        {
            UtilitiesMail.SendMail("ozanay.celik@btidanismanlik.com;onder.ozkum@btidanismanlik.com", subject: "Bulut Tahsilat Servisi",
                body: "<html><body><table><tr><td>Sayın İlgili,</td></tr><tr><td>Bulut entegrasyon servisi durdu.</td></tr></table></body></html>");
        }

        protected override void OnShutdown()
        {
            UtilitiesMail.SendMail("ozanay.celik@btidanismanlik.com;onder.ozkum@btidanismanlik.com", subject: "Bulut Tahsilat Servisi",
                body: "<html><body><table><tr><td>Sayın İlgili,</td></tr><tr><td>Bulut entegrasyon servisi durduruldu.</td></tr></table></body></html>");
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        public void CreateTable()
        {
            const string sqlQuery = @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES T WHERE T.TABLE_NAME = 'BTI_TRANSFER_ERRORLOG')
                    BEGIN
                        CREATE TABLE BTI_TRANSFER_ERRORLOG
                        (
                        LREF INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
                        PAYMENTID INT NULL,
                        PROCDATE DATETIME NULL,
                        FICHEDATE DATETIME NULL,
                        PAYMENTEXPCODE VARCHAR(50) NULL,
                        BULUTFICHETYPE VARCHAR(50) NULL,
                        LOGOFICHETYPE VARCHAR(50) NULL,
                        SENDERFIRMNAME VARCHAR(500) NULL,
                        SENDERBANK VARCHAR(50) NULL,
                        SENDERIBAN VARCHAR(50) NULL,
                        BRANCHFIRMNAME VARCHAR(500) NULL,
                        FIRMBANK VARCHAR(50) NULL,
                        FIRMIBAN VARCHAR(50) NULL,
                        EXPLANATION VARCHAR(2500) NULL,
                        PAYMENTDATA VARCHAR(MAX) NULL,
                        ERROR VARCHAR(MAX) NULL,
                        ERPTRANSFEREXP VARCHAR(1000)NULL,
                        ERPFICHENO VARCHAR(50)NULL,
                        ERPRESPONSE VARCHAR(MAX) NULL,
                        STATUSINFO varchar(2500) null,
                        )
                        IF OBJECT_ID('BTI_TRANSFER_ERRORLOG') IS NOT NULL SELECT 1 ISOK ELSE SELECT 0 ISOK
                    END 
                    ELSE
					SELECT 'BTI_TRANSFER_ERRORLOG Table already exists' RESULT";
            var connectionString = ConfigHelper.ConnectionString();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        var result = command.ExecuteScalar();

                        if (result.Equals(1))
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), " ", nameof(BulutTahsilatService),
                                " ", MethodBase.GetCurrentMethod().Name, " ",
                                "BTI_TRANSFER_ERRORLOG tablosu oluşturuldu!", Environment.NewLine, " [SORGU]  ", sqlQuery));

                            LogHelper.Log("<!----------------------Service Running------------------------------!>");
                        }
                        else if (result.ToString().Contains("exists"))
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), " ", nameof(BulutTahsilatService), " ", MethodBase.GetCurrentMethod().Name, " ",
                                "BTI_TRANSFER_ERRORLOG mevcut olduğundan yeniden oluşturulmadı...", Environment.NewLine, " [SORGU]  ", sqlQuery));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string.Concat(LogHelper.LogType.Error.ToLogType(), " ", nameof(BulutTahsilatService), " ", MethodBase.GetCurrentMethod().Name, " ",
                    "BTI_TRANSFER_ERRORLOG tablosu oluşturulamadı!", exception.Message, Environment.NewLine, " [SORGU]  ", sqlQuery);
            }
        }
        public void CreateFunction()
        {
            var config = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath);
            string getBankInfos = $@" --IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'fn_getBankInfos') )
                                      --SELECT 'fn_getBankInfos function already exists' RESULT
                                      --ELSE
                                        CREATE FUNCTION fn_getBankInfos(@TAXNR VARCHAR(51))
                                        RETURNS TABLE
                                        AS
                                        RETURN(
                                        SELECT BRANCH.LOGICALREF,BRANCH.TAXNR,BRANCH.CODE,BRANCH.BRANCH,BANKNAMES.BANKNAMES,BANKACCOUNT,BANKIBAN.BANKIBAN FROM
                                        (
                                        SELECT ROW_NUMBER() OVER(ORDER BY LOGICALREF) AS 'BRANCHROW', BRANCH.LOGICALREF,BRANCH.CODE,BRANCH.BRANCHCOLUMNNAME,BRANCH.BRANCH,BRANCH.TAXNR FROM 
                                        (  SELECT LOGICALREF,CODE,CONVERT(VARCHAR(51),TAXNR) AS 'TAXNR',BANKBRANCHS1,BANKBRANCHS2,BANKBRANCHS3,BANKBRANCHS4,BANKBRANCHS5,BANKBRANCHS6,BANKBRANCHS7 FROM LG_{config.LogoFirmNumber}_CLCARD 
                                          WHERE CARDTYPE <> 4 AND TAXNR = @TAXNR
                                        ) PVT
                                        UNPIVOT
                                        (  BRANCH FOR BRANCHCOLUMNNAME IN(BANKBRANCHS1,BANKBRANCHS2,BANKBRANCHS3,BANKBRANCHS4,BANKBRANCHS5,BANKBRANCHS6,BANKBRANCHS7)) BRANCH ) BRANCH
                                        LEFT JOIN
                                        (
                                        SELECT ROW_NUMBER() OVER(ORDER BY LOGICALREF) AS 'BANKNAMESROW',BANKNAMES.LOGICALREF,BANKNAMES.CODE,BANKNAMES.BANKNAMECOLUMNNAME,BANKNAMES.BANKNAMES FROM (
                                          SELECT  LOGICALREF,CODE,CONVERT(VARCHAR(51),TAXNR) AS 'TAXNR',BANKNAMES1,BANKNAMES2,BANKNAMES3,BANKNAMES4,BANKNAMES5,BANKNAMES6,BANKNAMES7 FROM LG_{config.LogoFirmNumber}_CLCARD 
                                          WHERE CARDTYPE <> 4 AND TAXNR = @TAXNR
                                        ) PVT
                                        UNPIVOT
                                        (
                                          BANKNAMES FOR BANKNAMECOLUMNNAME IN(BANKNAMES1,BANKNAMES2,BANKNAMES3,BANKNAMES4,BANKNAMES5,BANKNAMES6,BANKNAMES7)
                                        ) BANKNAMES
                                        ) BANKNAMES
                                        ON BRANCH.BRANCHROW = BANKNAMES.BANKNAMESROW 
                                        LEFT JOIN (
                                        SELECT ROW_NUMBER() OVER(ORDER BY LOGICALREF) AS 'BANKACCOUNTROW', BANKACCOUNT.BANKACCOUNT FROM (
                                          SELECT  LOGICALREF,CODE,CONVERT(VARCHAR(51),TAXNR) AS 'TAXNR',BANKACCOUNTS1,BANKACCOUNTS2,BANKACCOUNTS3,BANKACCOUNTS4,BANKACCOUNTS5,BANKACCOUNTS6,BANKACCOUNTS7 FROM  LG_{config.LogoFirmNumber}_CLCARD 
                                            WHERE CARDTYPE <> 4 AND TAXNR = @TAXNR
                                        ) PVT
                                        UNPIVOT
                                        (
                                          BANKACCOUNT FOR BANKACCOUNTCOLUMNNAME IN(BANKACCOUNTS1,BANKACCOUNTS2,BANKACCOUNTS3,BANKACCOUNTS4,BANKACCOUNTS5,BANKACCOUNTS6,BANKACCOUNTS7)
                                        ) BANKACCOUNT 
                                        ) BANKACCOUNT ON BANKACCOUNT.BANKACCOUNTROW = BRANCH.BRANCHROW
                                        LEFT JOIN (
                                        SELECT ROW_NUMBER() OVER(ORDER BY LOGICALREF) AS 'BANKIBANROW', BANKIBAN.BANKIBAN FROM (
                                          SELECT  LOGICALREF,CODE,CONVERT(VARCHAR(51),TAXNR) AS 'TAXNR',BANKIBANS1,BANKIBANS2,BANKIBANS3,BANKIBANS4,BANKIBANS5,BANKIBANS6,BANKIBANS7 FROM LG_{config.LogoFirmNumber}_CLCARD 
                                            WHERE CARDTYPE <> 4 AND TAXNR = @TAXNR
                                        ) PVT
                                        UNPIVOT
                                        (
                                          BANKIBAN FOR COLUMNNAME IN(BANKIBANS1,BANKIBANS2,BANKIBANS3,BANKIBANS4,BANKIBANS5,BANKIBANS6,BANKIBANS7)
                                        ) BANKIBAN ) BANKIBAN ON BANKIBAN.BANKIBANROW = BRANCH.BRANCHROW)";
            var connectionString = ConfigHelper.ConnectionString();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(getBankInfos, connection))
                    {
                        var result = command.ExecuteScalar();

                        if (result.Equals(1))
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), " ", nameof(BulutTahsilatService),
                                " ", MethodBase.GetCurrentMethod().Name, " ",
                                "fn_getBankInfos tablosu oluşturuldu!", Environment.NewLine, " [SORGU]  ", getBankInfos));

                            LogHelper.Log("<!----------------------Service Running------------------------------!>");
                        }
                        else if (result.ToString().Contains("exists"))
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), " ", nameof(BulutTahsilatService), " ", MethodBase.GetCurrentMethod().Name, " ",
                                "fn_getBankInfos mevcut olduğundan yeniden oluşturulmadı...", Environment.NewLine, " [SORGU]  ", getBankInfos));
                        }
                        Scheduler.StartJob().GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception exception)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), " ", nameof(BulutTahsilatService), " ", MethodBase.GetCurrentMethod().Name, " ",
                    "fn_getBankInfos fonksiyonu oluşturulamadı!", exception.Message, Environment.NewLine, " [SORGU]  ", getBankInfos));
            }
        }
    }
}
