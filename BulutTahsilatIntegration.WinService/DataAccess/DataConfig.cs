using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.Utilities;

namespace BulutTahsilatIntegration.WinService.DataAccess
{
    public class DataConfig
    {
        private static string _error;
        private static WinService.DataAccess.DataAccess _dataAccess = new WinService.DataAccess.DataAccess(ConfigHelper.ConnectionString(), ref _error);
        private static DataAccessException exception;

        public static bool CheckAndCreateDBObjects(out string message)
        {
            bool flag = true;
            message = string.Empty;
            //if (!_dataAccess.CheckIfTableExists("BTI_USERS"))
            //{
            //    CreateTable("BTI_USERS", out message);
            //    LogHelper.LogError(message);

            //}
            if (!_dataAccess.CheckIfTableExists("BTI_POSTLOG"))
            {
                CreateTable("BTI_POSTLOG", out message);
                LogHelper.LogError(message);
            }
            if (!_dataAccess.CheckIfTableExists("BTI_RESPONSELOG"))
            {
                CreateTable("BTI_RESPONSELOG", out message);
                LogHelper.LogError(message);
            }
            if (!_dataAccess.CheckIfTableExists("BTI_ERRORLOG"))
            {
                CreateTable("BTI_ERRORLOG", out message);
                LogHelper.LogError(message);
            }
            //if (!_dataAccess.CheckIfTableExists("BTI_GETLOG"))
            //{
            //    CreateTable("BTI_GETLOG", out message);
            //    LogHelper.LogError(message);
            //}
            return flag;
        }
        private static bool CreateTable(string tableName, out string message)
        {
            bool flag = true;
            message = string.Empty;
            string empty;
            if (tableName == "BTI_USERS")
            {
                empty = "CREATE TABLE BTI_USERS (LREF int? NOT NULL IDENTITY(1,1) PRIMARY KEY, COMPANY VARCHAR(50) NULL,USERNAME VARCHAR(50) NULL,PASSWORD VARCHAR(50) NULL,EMAIL VARCHAR(50) NULL)";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            else if (tableName == "BTI_POSTLOG")
            {
                empty = @"CREATE TABLE BTI_POSTLOG(LREF int PRIMARY KEY IDENTITY(1,1) NOT NULL,HOSTIP varchar(15) NULL,POSTDATE datetime NULL,
                OPERATIONTYPE varchar(50) NULL,IDENTITYNAME varchar(50) NULL,URL varchar(150) NULL,
                REQUESTMETHOD varchar(10) NULL,JSONDATA nvarchar(max) NULL)";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            else if (tableName == "BTI_RESPONSELOG")
            {
                empty = @"CREATE TABLE BTI_RESPONSELOG(LREF int PRIMARY KEY IDENTITY(1,1) NOT NULL,POSTREF int NULL,HOSTIP varchar(15) NULL,POSTDATE datetime NULL,
                OPERATIONTYPE varchar(50) NULL,IDENTITYNAME varchar(50) NULL,RESPONSESTATUS bit NULL,JSONDATA nvarchar(max) NOT NULL, RESPONSEDATA nvarchar(max) NULL)";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            else if (tableName == "BTI_ERRORLOG")
            {
                empty = "CREATE TABLE BTI_ERRORLOG(LREF int? NOT NULL IDENTITY(1,1) PRIMARY KEY,HOSTIP VARCHAR(15) NULL,POSTDATE DATETIME NULL,OPERATIONTYPE VARCHAR(50) NULL,IDENTITYNAME VARCHAR(50) NULL,ERRORCLASSNAME VARCHAR(50) NULL,ERRORMETHODNAME VARCHAR(50) NULL,ERRORMESSAGE VARCHAR(MAX) NULL,INNEREXCEPTION VARCHAR(MAX) NULL,JSONDATA NVARCHAR(MAX) NULL,RESPONSEDATA NVARCHAR(MAX) NULL)";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            else if (tableName == "BTI_GETLOG")
            {
                empty = @"CREATE TABLE BTI_GETLOG(LREF int IDENTITY(1,1) PRIMARY KEY NOT NULL,PROCDATE datetime NULL,OPERATIONNAME nvarchar(50) NULL,PAYMENTSTATUSTYPEID int NULL,BEGDATE datetime NULL,ENDDATE datetime NULL,METHODNAME nvarchar(250) NULL, FILTERS nvarchar(250) NULL,RAWDATA nvarchar(max) NULL,FILTEREDDATA nvarchar(max) NULL)";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            else if (tableName == "BTI_TRANSFER_ERRORLOG")
            {
                empty = @"CREATE TABLE BTI_TRANSFER_ERRORLOG(LREF int PRIMARY KEY IDENTITY(1,1) NOT NULL,PAYMENTID int NULL,PROCDATE datetime NULL,FICHEDATE datetime NULL
                            ,PAYMENTEXPCODE varchar(50) NULL,BULUTFICHETYPE varchar(50) NULL,LOGOFICHETYPE varchar(50) NULL
                            ,SENDERFIRMNAME varchar(500) NULL,SENDERBANK varchar(50) NULL,SENDERIBAN varchar(50) NULL
                            ,BRANCHFIRMNAME varchar(500) NULL,FIRMBANK varchar(50) NULL,FIRMIBAN varchar(50) NULL
                            ,EXPLANATION varchar(2500) NULL,PAYMENTDATA varchar(max) NULL,ERROR varchar(max) NULL
                            ,ERPTRANSFEREXP varchar(1000) NULL,ERPFICHENO varchar(50) NULL,ERPRESPONSE varchar(max) NULL
                            ,ERPPOSTJSON varchar(max) NULL,STATUSINFO varchar(max) NULL )";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            else if (tableName == "BTI_BULUTBANKBALANCE")
            {
                empty = @"CREATE TABLE BTI_BULUTBANKBALANCE(LREF INT PRIMARY KEY IDENTITY(1,1)  NOT NULL,BANKCODE VARCHAR(50) NULL,BANKNAME VARCHAR(50) NULL,FIRMNAME VARCHAR(50) NULL,BANKBRANCHNAME VARCHAR(50) NULL,BANKNICKNAME VARCHAR(50) NULL,BANKACCOUNTTYPE VARCHAR(50) NULL,BANKIBAN VARCHAR(50) NULL,BANKCURRENCYUNIT VARCHAR(50) NULL,LASTTIMESTMAP DATETIME NULL,BALANCE DECIMAL NULL,BLOCKEDBALANCE DECIMAL)";
                flag = _dataAccess.ExecuteScalar(empty.ToReplaceLogoTableName(), ref exception).ToBool();
            }
            if (!string.IsNullOrEmpty(message))
            {
                flag = false;
            }
            return flag;
        }
    }
}
