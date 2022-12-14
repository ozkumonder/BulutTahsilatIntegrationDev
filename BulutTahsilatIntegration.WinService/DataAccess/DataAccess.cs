using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataSet = System.Data.DataSet;
using DataTable = System.Data.DataTable;

namespace BulutTahsilatIntegration.WinService.DataAccess
{
    public class DataAccess
    {
        private static DataAccessException exception;
        public SqlConnection Connection
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }
        public bool Exists(string sqlQuery)
        {
            string empty = string.Empty;
            bool flag = Convert.ToBoolean(this.ExecuteScalar(sqlQuery, ref exception));
            return flag;
        }
        public bool CheckColumnExist(string columnName, string tableName)
        {
            bool flag = this.Exists($"IF EXISTS(select * from sys.columns where Name ='{columnName}' and Object_ID = Object_ID('{tableName}'))                                      select 'true' else SELECT 'false'");
            return flag;
        }

        public bool CheckIfDbExists(string dbName)
        {
            return this.Exists($"IF EXISTS(SELECT * FROM master.dbo.sysdatabases where name ='{dbName}') select 'true' else SELECT 'false'");
        }

        public bool CheckIfFunctionExist(string functionName)
        {
            return this.Exists(
                $"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = '{functionName}') SELECT 'true' ELSE SELECT 'false'");
        }

        public bool CheckIfSPExist(string spName)
        {
            return this.Exists(
                $"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{spName}') SELECT 'true' ELSE SELECT 'false'");
        }

        public bool CheckIfTableExists(string tableName)
        {
            return this.Exists($"IF OBJECT_ID('{tableName}', 'U') IS NOT NULL SELECT 'true' ELSE SELECT 'false'");
        }

        public bool CheckIndexExists(string pTableName, string pIndexName)
        {
            bool flag = this.Exists(string.Concat("SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID(N", this.QuotedStr(pTableName), ") AND name = N", this.QuotedStr(pIndexName)));
            return flag;
        }
        private string QuotedStr(string text)
        {
            return string.Concat("'", text, "'");
        }
        public DataAccess(string connectionString, ref string error)
        {
            ConnectionString = connectionString;
            try
            {
                Connection = new SqlConnection(connectionString);
                Connection.Open();
            }
            catch (Exception exception)
            {
                error = exception.Message;
            }
        }
        #region Non Query

        public int? ExecuteNonQuery(string commandText, ref DataAccessException exception)
        {
            int? result;
            commandText = string.Concat(commandText, "  ; SELECT ISNULL(SCOPE_IDENTITY(),0) AS LASTID ");
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                var sqlCommand = new SqlCommand(commandText, Connection);
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var sqlException = ex;
                exception.ErrorNr = sqlException.HResult;
                exception.ErrorDesc = sqlException.Message;
                result = 0;
            }
            return result;
        }
        public int? ExecuteNonQueryWithParams(string commandText, SqlParameter[] sqlParameter, ref DataAccessException exception)
        {
            int? result;
            commandText = string.Concat(commandText, "  ; SELECT ISNULL(SCOPE_IDENTITY(),0) AS LASTID ");
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                var sqlCommand = new SqlCommand(commandText, Connection);
                if (sqlParameter != null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameter);
                }
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var sqlException = ex;
                exception.ErrorNr = sqlException.HResult;
                exception.ErrorDesc = sqlException.Message;
                result = 0;
            }
            return result;
        }

        public void ExecuteNonQueryNoLastID(string commandText, ref DataAccessException exception)
        {
            ExecuteNonQuery(commandText, ref exception);
        }

        public void ExecuteNonQueryWithStoredProcedure(string spName, DbParameter[] dbParameter, ref DataAccessException exception)
        {
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                var sqlCommand = new SqlCommand(spName, Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddRange(dbParameter);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var sqlException = ex;
                exception.ErrorNr = sqlException.HResult;
                exception.ErrorDesc = sqlException.Message;
            }
        }

        #endregion

        #region Scalar

        public object ExecuteScalar(string commandText, ref DataAccessException exception)
        {
            object obj;
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                obj = (new SqlCommand(commandText, Connection)).ExecuteScalar();
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;
                exception.ErrorNr = exception1.HResult;
                exception.ErrorDesc = exception1.Message;
                obj = null;
            }
            return obj;
        }

        public object ExecuteScalar(string commandText, SqlParameter[] sqlParameter, ref DataAccessException exception)
        {
            object obj;
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                SqlCommand sqlCommand = new SqlCommand(commandText, Connection);
                if (sqlParameter != null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameter);
                }
                obj = sqlCommand.ExecuteScalar();
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;
                exception.ErrorNr = exception1.HResult;
                exception.ErrorDesc = exception1.Message;
                obj = null;
            }
            return obj;
        }

        #endregion

        #region GetDataTable

        public DataTable GetDataTable(string commandText, ref DataAccessException exception)
        {
            return GetDataTableByParams(commandText, (SqlParameter[])null, ref exception);
        }

        public DataTable GetDataTableByParams(string commandText, DbParameter[] dbParameter, ref DataAccessException exception)
        {
            DataTable dataTable = new DataTable();
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                SqlCommand sqlCommand = new SqlCommand(commandText, Connection);
                if (dbParameter != null)
                {
                    sqlCommand.Parameters.AddRange(dbParameter);
                }
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataTable);
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;
                exception.ErrorDesc = exception1.Message;
                exception.ErrorNr = exception1.HResult;
                dataTable = null;
            }
            return dataTable;
        }

        public DataTable GetDataTableByParams(string commandText, SqlParameter[] sqlParameter, ref DataAccessException exception)
        {
            DbParameter[] dbParameterArray = null;
            if (sqlParameter != null)
            {
                dbParameterArray = new DbParameter[(int)sqlParameter.Length];
            }
            return GetDataTableByParams(commandText, (DbParameter[])sqlParameter, ref exception);
        }

        #endregion

        #region GetDataTable

        public DataTable GetDataSet(string commandText, ref DataAccessException exception)
        {
            return GetDataSetByParams(commandText, (SqlParameter[])null, ref exception);
        }

        public DataSet GetDataSetByParams(string commandText, DbParameter[] dbParameter, ref DataAccessException exception)
        {
            DataSet result = new DataSet();
            try
            {
                exception = new DataAccessException(0);
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                SqlCommand sqlCommand = new SqlCommand(commandText, Connection);
                if (dbParameter != null)
                {
                    sqlCommand.Parameters.AddRange(dbParameter);
                }
                //SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                //sqlDataAdapter.Fill(result);
                var adp = DbProviderFactories.GetFactory(Connection).CreateDataAdapter();
                adp.SelectCommand = sqlCommand;
                adp.Fill(result);
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;
                exception.ErrorDesc = exception1.Message;
                exception.ErrorNr = exception1.HResult;
                result = null;
            }
            return result;
        }

        public DataTable GetDataSetByParams(string commandText, SqlParameter[] sqlParameter, ref DataAccessException exception)
        {
            DbParameter[] dbParameterArray = null;
            if (sqlParameter != null)
            {
                dbParameterArray = new DbParameter[(int)sqlParameter.Length];
            }
            return GetDataTableByParams(commandText, (DbParameter[])sqlParameter, ref exception);
        }

        #endregion



    }

}
