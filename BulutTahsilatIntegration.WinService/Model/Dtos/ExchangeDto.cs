using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulutTahsilatIntegration.WinService.Model.Dtos
{
    public class ExchangeDto
    {
        public int? LREF { get; set; }
        public DateTime DATE_ { get; set; }
        public double RATES1 { get; set; }
        public double RATES2 { get; set; }
        public double RATES3 { get; set; }
        public double RATES4 { get; set; }
        public DateTime EDATE { get; set; }
    }
}
