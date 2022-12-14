using System;

namespace BulutTahsilatIntegration.WinService.DataAccess
{
    public class DataAccessException : Exception
    {
        public string ErrorDesc;

        public int? ErrorNr
        {
            get;
            set;
        }

        public DataAccessException(int? errorNr)
        {
            this.ErrorNr = errorNr;
            this.ErrorDesc = "";
        }
    }
}
