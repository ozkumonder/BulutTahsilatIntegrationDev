using Newtonsoft.Json.Linq;

namespace BulutTahsilatIntegration.WinService.Model.ResultTypes
{
    public class ServiceResult
    {
        public int LogRef
        {
            get;
            set;
        }
        public object ObjectNo
        {
            get;
            set;
        }

        public bool Success
        {
            get;
            set;
        }
        public int? RowCount
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public int? ErrorCode
        {
            get;
            set;
        }

        public string ErrorDesc
        {
            get;
            set;
        }
        public JToken JResult { get; set; }
    }
}
