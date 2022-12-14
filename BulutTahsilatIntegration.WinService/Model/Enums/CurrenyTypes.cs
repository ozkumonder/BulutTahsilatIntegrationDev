using System.Collections.Generic;

namespace BulutTahsilatIntegration.WinService.Model.Enums
{
    public class CurrenyTypes
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public static List<CurrenyTypes> GetCurrenyTypes()
        {
            var list = new List<CurrenyTypes>
            {
                new CurrenyTypes { Key = "TL",Value = 0},
                new CurrenyTypes { Key = "TRY",Value = 0},
                new CurrenyTypes { Key = "USD",Value = 1},
                new CurrenyTypes { Key = "EUR",Value = 20},
                new CurrenyTypes { Key = "GBP",Value = 17},
            };
            return list;
        }
    }
}
