using System.Security.AccessControl;

namespace BulutTahsilatIntegration.WinService.Model
{
    /// <summary>
    /// Cari Hesap Kartı
    /// </summary>
    public class ClientCard
    {
        public int Id { get; set; }
        public string ClientCode { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string CityID { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string AccountingCode { get; set; }
        public string PaymentExpCode { get; set; }
    }
}