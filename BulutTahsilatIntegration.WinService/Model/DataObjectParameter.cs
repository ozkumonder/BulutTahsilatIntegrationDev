using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulutTahsilatIntegration.WinService.Model
{
    public class DataObjectParameter
    {
        public DataObjectParameter()
        {
            this.ReplicMode = false;
            this.CheckParams = false;
            this.CheckRight = false;
            this.Validation = true;
            this.FormSeriLotLinesOnPreSave = false;
            this.ApplyCampaignOnPreSave = false;
            this.ApplyConditionOnPreSave = false;
            this.FillAccCodesOnPreSave = true;
        }
        public bool ReplicMode { get; set; }
        public bool CheckParams { get; set; }
        public bool CheckRight { get; set; }
        public bool Validation { get; set; }
        public bool CheckApproveDate { get; set; }
        public bool ApplyCampaignOnPreSave { get; set; }
        public bool ApplyConditionOnPreSave { get; set; }
        public bool FormSeriLotLinesOnPreSave { get; set; }
        public bool FillAccCodesOnPreSave { get; set; }
    }
}
