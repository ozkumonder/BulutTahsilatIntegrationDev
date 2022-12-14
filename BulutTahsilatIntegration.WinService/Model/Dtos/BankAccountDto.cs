namespace BulutTahsilatIntegration.WinService.Model.Dtos
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public int BnCardType { get; set; }
        public int BankRef { get; set; }
        public string BankAccountCode { get; set; }
        public string Iban { get; set; }
        public short CurrencyType { get; set; }
        public string CurrencyCode { get; set; }
    }
}
