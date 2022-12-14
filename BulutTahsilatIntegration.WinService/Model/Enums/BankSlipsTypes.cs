using System.Collections.Generic;
using System.IO.Packaging;

namespace BulutTahsilatIntegration.WinService.Model.Enums
{
    public enum BankSlipsTypes
    {
        IslemFisi = 1,
        VirmanFisi = 2,
        BankaGelenHavale = 3,
        GonderilenHavale = 4,
        AcilisFisi = 5,
        KurFarkiFisi = 6,
        DovizAlisBelgesi = 7,
        DovizSatisBelgesi = 8,
        BankaAlinanHizmetFaturasi = 16,
        BankaVerilenHizmetFaturasi = 17,
        BankadanCekOdemesi = 18,
        BankadanSenetOdemesi = 19,
        BankadanGiderPusulasi = 20,
        BankadanMüstahsilMakbuzu = 21
    }

    public enum PaymentTypeID
    {
        Kredi = 512,
        BankaHareketi = 513,
        Nakit = 514,
        Virman = 515,
        Cek = 516,
        Pos = 517,
        Masraf = 518,
        Senet = 519
    }

    public enum PaymentStatusTypeID
    {
        EslesmeYapildi = 531,
        Tamamlandi = 532,
        IptalEdildi = 533,
        EslesmeYapilmadi = 534
    }

    
}
