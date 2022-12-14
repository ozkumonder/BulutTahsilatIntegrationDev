namespace BulutTahsilatIntegration.WinService.BTIIntegration
{
    public class TigerDataType
    {
        private TigerDataType(string value) { Value = value; }
        public string Value { get; set; }
        /// <summary>
        /// Malzeme Kartý
        /// </summary>
        public static TigerDataType Items => new TigerDataType("items"); 
        /// <summary>
        /// Malzeme Hareketleri
        /// </summary>
        public static TigerDataType ItemSlips => new TigerDataType("itemSlips");
        /// <summary>
        /// Alýnan Hizmet Kartý
        /// </summary>
        public static TigerDataType PurchasedServices => new TigerDataType("purchasedServices");
        /// <summary>
        /// Satýþ Sipariþi
        /// </summary>
        public static TigerDataType SalesOrders => new TigerDataType("salesOrders");
        /// <summary>
        /// Alým Sipariþi
        /// </summary>
        public static TigerDataType PurchaseOrders => new TigerDataType("purchaseOrders");
        /// <summary>
        /// Alým Ýndirim Kartý
        /// </summary>
        public static TigerDataType PurchaseDiscounts => new TigerDataType("purchaseDiscounts");
        /// <summary>
        /// Alým masraf kartý
        /// </summary>
        public static TigerDataType PurchaseExpenses => new TigerDataType("purchaseExpenses");
        /// <summary>
        /// Satýþ Ýndirim Kartý
        /// </summary>
        public static TigerDataType SalesDiscounts => new TigerDataType("salesDiscounts");
        /// <summary>
        /// Satýþ Masraf Kartý
        /// </summary>
        public static TigerDataType SalesExpenses => new TigerDataType("salesExpenses");
        /// <summary>
        /// Alým Promosyon
        /// </summary>
        public static TigerDataType PurchasePromotions => new TigerDataType("purchasePromotions");
        /// <summary>
        /// Satýs Promosyon
        /// </summary>
        public static TigerDataType SalesPromotions => new TigerDataType("salesPromotions");
        /// <summary>
        /// Mal Alým Fiyat Kartý
        /// </summary>
        public static TigerDataType PurchasedItemPrices => new TigerDataType("purchasedItemPrices");
        /// <summary>
        /// Hizmet Alým Fiyat Kartý
        /// </summary>
        public static TigerDataType PurchasedServicePrices => new TigerDataType("purchasedServicePrices");
        /// <summary>
        /// Malzeme Satýþ Fiyatý
        /// </summary>
        public static TigerDataType SalesItemPrices => new TigerDataType("salesItemPrices");
        /// <summary>
        /// Hizmet Satýþ Fiyatý
        /// </summary>
        public static TigerDataType SalesServicePrices => new TigerDataType("salesServicePrices");
        /// <summary>
        /// Satýcý-Cari Baðlatýsý
        /// </summary>
        public static TigerDataType Salesmen => new TigerDataType("salesmen");
        /// <summary>
        /// Alým Ýrsaliye
        /// </summary>
        public static TigerDataType PurchaseDispatches => new TigerDataType("purchaseDispatches");
        /// <summary>
        /// Satýþ Ýrsaliye
        /// </summary>
        public static TigerDataType SalesDispatches => new TigerDataType("salesDispatches");
        /// <summary>
        /// Alým Fatura
        /// </summary>
        public static TigerDataType PurchaseInvoices => new TigerDataType("purchaseInvoices");
        /// <summary>
        ///  Satýþ Fatura
        /// </summary>
        public static TigerDataType SalesInvoices => new TigerDataType("salesInvoices");
        /// <summary>
        ///  Çek/Senet Devir
        /// </summary>
        public static TigerDataType ChequeAndPnotes => new TigerDataType("chequeAndPnotes");
        /// <summary>
        ///  Çek/Senet Bordorsu
        /// </summary>
        public static TigerDataType ChequeAndPnoteRolls => new TigerDataType("chequeAndPnoteRolls");
        /// <summary>
        ///  Banka Kartý
        /// </summary>
        public static TigerDataType Banks => new TigerDataType("banks");
        /// <summary>
        ///  Banka Hesap Kartý
        /// </summary>
        public static TigerDataType BankAccounts => new TigerDataType("bankAccounts");
        /// <summary>
        ///  Banka Fiþi
        /// </summary>
        public static TigerDataType BankSlips => new TigerDataType("bankSlips");
        /// <summary>
        ///  Muhasebe Hesap Kartý
        /// </summary>
        public static TigerDataType GLAccounts => new TigerDataType("gLAccounts");
        /// <summary>
        ///  Muhasebe Fiþi
        /// </summary>
        public static TigerDataType GLSlips => new TigerDataType("gLSlips");
        /// <summary>
        ///  Masraf Merkezi
        /// </summary>
        public static TigerDataType OverheadAccounts => new TigerDataType("overheadAccounts");
        /// <summary>
        ///  Kasa Kartlarý
        /// </summary>
        public static TigerDataType SafeDeposits => new TigerDataType("safeDeposits");
        /// <summary>
        ///  Kasa Ýþlemleri
        /// </summary>
        public static TigerDataType SafeDepositSlips => new TigerDataType("safeDepositSlips");
        /// <summary>
        ///  Cari Kart
        /// </summary>
        public static TigerDataType Arps => new TigerDataType("arps");
        /// <summary>
        ///  Cari Fiþ
        /// </summary>
        public static TigerDataType ArpSlips => new TigerDataType("arpSlips");
        /// <summary>
        ///  Ödeme Planý
        /// </summary>
        public static TigerDataType PaymentPlans => new TigerDataType("paymentPlans");
        /// <summary>
        ///  Birim Seti
        /// </summary>
        public static TigerDataType UnitSets => new TigerDataType("unitSets");
        /// <summary>
        ///  Cari Sevkiyat Adresleri
        /// </summary>
        public static TigerDataType ArpShipmentLocations => new TigerDataType("arpShipmentLocations");
        /// <summary>
        ///  Sabit Kýymet Kaydý
        /// </summary>
        public static TigerDataType FaRegistries => new TigerDataType("faRegistries");
        /// <summary>
        ///  Malzeme Farklý Dilde Açýklamalar
        /// </summary>
        public static TigerDataType ItemMLDescriptions => new TigerDataType("itemMLDescriptions");
        /// <summary>
        ///  Cari Kart Farklý Dil Açýklamalarý
        /// </summary>
        public static TigerDataType ArpMLDescriptons => new TigerDataType("ArpMLDescriptons");
        /// <summary>
        ///  Banka Kartý Farklý Dilde Açýklamalar
        /// </summary>
        public static TigerDataType BankMLDescriptions => new TigerDataType("bankMLDescriptions");
        /// <summary>
        /// Hesap Kartý Farklý Dil Açýklamalar
        /// </summary>
        public static TigerDataType GLAccountMLDescriptions => new TigerDataType("glAccountMLDescriptions");
        /// <summary>
        /// Müþteri Yabancý Dil Açýklamalarý
        /// </summary>
        public static TigerDataType CustomerMLDescriptions => new TigerDataType("customerMLDescriptions");
        /// <summary>
        /// Alternatifler
        /// </summary>
        public static TigerDataType ItemAlternatives => new TigerDataType("	itemAlternatives");
        /// <summary>
        /// Malzeme Reçetesi
        /// </summary>
        public static TigerDataType ItemBoms => new TigerDataType("itemBoms");
        /// <summary>
        /// Seri Lot Tablosu
        /// </summary>
        public static TigerDataType SerialAndLotNumbers => new TigerDataType("serialAndLotNumbers");
        /// <summary>
        /// Malzeme Özellikleri
        /// </summary>
        public static TigerDataType ItemCharacteristics => new TigerDataType("itemCharacteristics");
        /// <summary>
        /// Ýþ Ýstasyonu Özellikleri
        /// </summary>
        public static TigerDataType Characteristics => new TigerDataType("characteristics");
        /// <summary>
        /// Ýþ Ýstasyonu
        /// </summary>
        public static TigerDataType Workstations => new TigerDataType("workstations");
        /// <summary>
        /// Ýþ Ýstasyonu Grubu
        /// </summary>
        public static TigerDataType WorkstationGroups => new TigerDataType("workstationGroups");
        /// <summary>
        /// Çalýþan
        /// </summary>
        public static TigerDataType Employees => new TigerDataType("employees");
        /// <summary>
        /// Çalýþan Gruplarý
        /// </summary>
        public static TigerDataType EmployeeGroups => new TigerDataType("employeeGroups");
        /// <summary>
        /// Ýþ Ýstasyonu Maliyetleri
        /// </summary>
        public static TigerDataType WorkstationCosts => new TigerDataType("workstationCosts");
        /// <summary>
        /// Çalýþan Maliyetleri
        /// </summary>
        public static TigerDataType EmployeeCosts => new TigerDataType("employeeCosts");
        /// <summary>
        /// Vardiya
        /// </summary>
        public static TigerDataType Shifts => new TigerDataType("shifts");
        /// <summary>
        /// Vardiya Atamasý
        /// </summary>
        public static TigerDataType ShiftAssignments => new TigerDataType("shiftAssignments");
        /// <summary>
        /// Ürün Reçetesi
        /// </summary>
        public static TigerDataType Boms => new TigerDataType("boms");
        /// <summary>
        /// Operasyonlar
        /// </summary>
        public static TigerDataType Operations => new TigerDataType("operations");
        /// <summary>
        /// Rota Tanýmý
        /// </summary>
        public static TigerDataType ProductionRoutes => new TigerDataType("productionRoutes");
        /// <summary>
        /// Reçete Sabitleri
        /// </summary>
        public static TigerDataType ProductionParameters => new TigerDataType("productionParameters");
        /// <summary>
        /// Kalite Kontrol Seti
        /// </summary>
        public static TigerDataType QCCriteriaSets => new TigerDataType("qccriteriaSets");
        /// <summary>
        /// Teslimat Kodu
        /// </summary>
        public static TigerDataType DeliveryCodes => new TigerDataType("deliveryCodes");
        /// <summary>
        /// Grup Kodlarý
        /// </summary>
        public static TigerDataType GroupCodes => new TigerDataType("groupCodes");
        /// <summary>
        /// Satýcý Pozisyon Kodlarý
        /// </summary>
        public static TigerDataType SalesmanPositionCodes => new TigerDataType("salesmanPositionCodes");
        /// <summary>
        /// Ödeme Planý Grup Kodu
        /// </summary>
        public static TigerDataType PaymentPlanGroupCodes => new TigerDataType("paymentPlanGroupCodes");
        /// <summary>
        /// Özel Kodlar
        /// </summary>
        public static TigerDataType SpecialCodes => new TigerDataType("specialCodes");
        /// <summary>
        /// Yetki Kodlarý
        /// </summary>
        public static TigerDataType AuthorizationCodes => new TigerDataType("authorizationCodes");
        /// <summary>
        /// Satýcý-Cari Baðlantýsý(Satýþ Yönetimi Flag’i Açýkken)
        /// </summary>
        public static TigerDataType CustomersOfSalesmen => new TigerDataType("customersOfSalesmen");
        /// <summary>
        /// Satýcý Rota Baglantýsý
        /// </summary>
        public static TigerDataType SalesmanRoutes => new TigerDataType("salesmanRoutes");
        /// <summary>
        /// Satýcý Hedef Baðlantýsý
        /// </summary>
        public static TigerDataType SalesmanDestinations => new TigerDataType("salesmanDestinations");
        /// <summary>
        /// Alým Kampanyasý
        /// </summary>
        public static TigerDataType PurchaseCampaigns => new TigerDataType("purchaseCampaigns");
        /// <summary>
        /// Satýþ Kampanya
        /// </summary>
        public static TigerDataType SalesCampaigns => new TigerDataType("salesCampaigns");
        /// <summary>
        /// Daðýtým Aracý
        /// </summary>
        public static TigerDataType Vehicles => new TigerDataType("vehicles");
        /// <summary>
        /// Daðýtým Rotasý
        /// </summary>
        public static TigerDataType DistributionRoutes => new TigerDataType("distributionRoutes");
        /// <summary>
        /// Daðýtým Emri
        /// </summary>
        public static TigerDataType DistributionOrders => new TigerDataType("distributionOrders");
        /// <summary>
        /// Ülke Bilgileri
        /// </summary>
        public static TigerDataType Countries => new TigerDataType("countries");
        /// <summary>
        /// Þehir Bilgileri
        /// </summary>
        public static TigerDataType Cities => new TigerDataType("cities");
        /// <summary>
        /// Posta Kodu Bilgisi
        /// </summary>
        public static TigerDataType PostCodes => new TigerDataType("postCodes");
        /// <summary>
        /// Ýlçe Bilgileri
        /// </summary>
        public static TigerDataType Towns => new TigerDataType("towns");
        /// <summary>
        /// Semt
        /// </summary>
        public static TigerDataType Districts => new TigerDataType("districts");
        /// <summary>
        /// Malzeme Sýnýfý Atamasý
        /// </summary>
        public static TigerDataType ItemClassAssignments => new TigerDataType("itemClassAssignments");
        /// <summary>
        /// Standart Maliyet Periodlarý
        /// </summary>
        public static TigerDataType StandardCostPeriods => new TigerDataType("standardCostPeriods");
        /// <summary>
        /// Malzeme Standart Maliyeti
        /// </summary>
        public static TigerDataType ItemStandardCosts => new TigerDataType("itemStandardCosts");
        /// <summary>
        /// Ýþ Ýstasyonu Standart Maliyeti
        /// </summary>
        public static TigerDataType WorkstationStandardCosts => new TigerDataType("workstationStandardCosts");
        /// <summary>
        /// Çalýþan Standart Maliyeti
        /// </summary>
        public static TigerDataType EmployeeStandardCosts => new TigerDataType("employeeStandardCosts");
        /// <summary>
        /// Standart Reçete Maliyeti
        /// </summary>
        public static TigerDataType BomStandardCosts => new TigerDataType("bomStandardCosts");
        /// <summary>
        /// Ýþ istasyonu Ýstisnalarý
        /// </summary>
        public static TigerDataType ProductionExceptions => new TigerDataType("productionExceptions");
        /// <summary>
        /// Verilen Hizmet Kartý
        /// </summary>
        public static TigerDataType SoldServices => new TigerDataType("soldServices");
        /// <summary>
        /// Ek Vergi Kartý
        /// </summary>
        public static TigerDataType AdditionalTaxes => new TigerDataType("additionalTaxes");
        /// <summary>
        /// Ürün Hattý
        /// </summary>
        public static TigerDataType ProductionLines => new TigerDataType("productionLines");
        /// <summary>
        /// Talep Karþýlama
        /// </summary>
        public static TigerDataType DemandPeggings => new TigerDataType("demandPeggings");
        /// <summary>
        /// Fiyat Farký Faturasý
        /// </summary>
        public static TigerDataType PaymentDifferenceInvoices => new TigerDataType("paymentDifferenceInvoices");
        /// <summary>
        /// Proje Kartý
        /// </summary>
        public static TigerDataType Projects => new TigerDataType("projects");
        /// <summary>
        /// Geri Ödeme Planlarý
        /// </summary>
        public static TigerDataType RepaymentPlans => new TigerDataType("repaymentPlans");
        /// <summary>
        /// Daðýtým Þablonlarý
        /// </summary>
        public static TigerDataType DistributionTemplates => new TigerDataType("distributionTemplates");
        /// <summary>
        /// Stok Yeri Kodlarý
        /// </summary>
        public static TigerDataType LocationCodes => new TigerDataType("locationCodes");
        /// <summary>
        /// Satýþ Koþullarý (Fiþ Satýrlarý)
        /// </summary>
        public static TigerDataType SalesConditionsForSlipLines => new TigerDataType("salesConditionsForSlipLines");
        /// <summary>
        /// Satýþ Koþullarý (Fiþ Geneli)
        /// </summary>
        public static TigerDataType SalesConditionsForSlips => new TigerDataType("salesConditionsForSlips");
        /// <summary>
        /// Alýþ Koþullarý (Fiþ Satýrlarý)
        /// </summary>
        public static TigerDataType PurchaseConditionsForSlipLines => new TigerDataType("purchaseConditionsForSlipLines");
        /// <summary>
        /// Alýþ Koþullarý (Fiþ Geneli) 
        /// </summary>
        public static TigerDataType PurchaseConditionsForSlips => new TigerDataType("purchaseConditionsForSlips");
        /// <summary>
        /// Talep
        /// </summary>
        public static TigerDataType DemandSlips => new TigerDataType("demandSlips");
        /// <summary>
        /// Ýhracat Kredisi (Döviz/Exim)
        /// </summary>
        public static TigerDataType ExportCredits => new TigerDataType("exportCredits");
        /// <summary>
        /// Serbest Bölge Tanýmý
        /// </summary>
        public static TigerDataType FreeZones => new TigerDataType("freeZones");
        /// <summary>
        /// Gümrük Tanýmý
        /// </summary>
        public static TigerDataType CustomsOffices => new TigerDataType("customsOffices");
        /// <summary>
        /// Ýthalat Operasyon Fiþi
        /// </summary>
        public static TigerDataType ImportOperationSlips => new TigerDataType("importOperationSlips");
        /// <summary>
        /// Ýhracat Operasyon Fiþi
        /// </summary>
        public static TigerDataType ExportOperationSlips => new TigerDataType("exportOperationSlips");
        /// <summary>
        /// Ýhracat / Ýhraç Kayýtlý Alým Faturalarý
        /// </summary>
        public static TigerDataType ExportTypedPurchaseInvoices => new TigerDataType("exportTypedPurchaseInvoices");
        /// <summary>
        /// Ýhracat / Ýhraç Kayýtlý Satýþ Faturalarý
        /// </summary>
        public static TigerDataType ExportTypedSalesInvoices => new TigerDataType("exportTypedSalesInvoices");
        /// <summary>
        /// Ýhracat / Dahilde Ýþleme Ýzin Belgesi
        /// </summary>
        public static TigerDataType InwardProcessingPermits => new TigerDataType("inwardProcessingPermits");
        /// <summary>
        /// Ýthalat / Malzeme Dolaþým Fiþi
        /// </summary>
        public static TigerDataType ExportMovementSlips => new TigerDataType("exportMovementSlips");
        /// <summary>
        /// Ýthalat / Millileþtirme Fiþleri
        /// </summary>
        public static TigerDataType ExportNationalizationSlips => new TigerDataType("exportNationalizationSlips");
        /// <summary>
        /// Ýthalat / Daðýtým Fiþleri
        /// </summary>
        public static TigerDataType ImportDistributionSlips => new TigerDataType("importDistributionSlips");
        /// <summary>
        /// Marka Tanýmlarý
        /// </summary>
        public static TigerDataType ItemBrands => new TigerDataType("itemBrands");
        /// <summary>
        /// Tanýmlý Alanlar
        /// </summary>
        public static TigerDataType ExtendedFields => new TigerDataType("extendedFields");
        /// <summary>
        /// Tanýmlý Alan Tanýmlarý
        /// </summary>
        public static TigerDataType ExtendedFieldDefinitions => new TigerDataType("extendedFieldDefinitions");
        /// <summary>
        /// Zorunlu Alanlar
        /// </summary>
        public static TigerDataType MandatoryFields => new TigerDataType("mandatoryFields");
        /// <summary>
        /// Tanýmlý Alan Kategori Listeleri
        /// </summary>
        public static TigerDataType ExtendedFieldCategories => new TigerDataType("extendedFieldCategories");
        /// <summary>
        /// Ýþ Akýþ Yönetimi  / AnaKayýtlar / Ýþ Akýþ Rol Tanýmlarý
        /// </summary>
        public static TigerDataType WorkflowRoles => new TigerDataType("workflowRoles");
        /// <summary>
        /// Ýþ Akýþ Yönetimi  / AnaKayýtlar / Ýþ Akýþ Kartlarý
        /// </summary>
        public static TigerDataType WorkflowDefinitions => new TigerDataType("workflowDefinitions");
        /// <summary>
        /// Planlanan/Gerçekleþen Kaynak Kullanýmý Giriþi
        /// </summary>
        public static TigerDataType ProductionResourceUtilization => new TigerDataType("productionResourceUtilization");
        /// <summary>
        /// Grup Þirketi
        /// </summary>
        public static TigerDataType ArpGroupAssignments => new TigerDataType("arpGroupAssignments");
        /// <summary>
        /// Teminat Bordrolarý
        /// </summary>
        public static TigerDataType CollateralRolls => new TigerDataType("collateralRolls");
        /// <summary>
        /// Satýnalma Teklif Yönetimi – Emir
        /// </summary>
        public static TigerDataType PurchaseProposalOrders => new TigerDataType("purchaseProposalOrders");
        /// <summary>
        /// Satýnalma Teklif Yönetimi – Teklif
        /// </summary>
        public static TigerDataType PurchaseProposalOffers => new TigerDataType("purchaseProposalOffers");
        /// <summary>
        /// Satýnalma Teklif Yönetimi – Sözleþme
        /// </summary>
        public static TigerDataType PurchaseProposalContracts => new TigerDataType("purchaseProposalContracts");
        /// <summary>
        /// Hýzlý Üretim Fiþi
        /// </summary>
        public static TigerDataType QuickProductionSlips => new TigerDataType("quickProductionSlips");
        /// <summary>
        /// Müþteriler (Teklif Yönetimi Kategorileri)
        /// </summary>
        public static TigerDataType Customers => new TigerDataType("customers");
        /// <summary>
        /// Satýþ Kategorileri
        /// </summary>
        public static TigerDataType SalesCategories => new TigerDataType("salesCategories");
        /// <summary>
        /// Ýlgili Kiþiler (Teklif Yönetimi Kategorileri)
        /// </summary>
        public static TigerDataType Contacts => new TigerDataType("contacts");
        /// <summary>
        /// Banka Kredileri
        /// </summary>
        public static TigerDataType BankCredits => new TigerDataType("bankCredits");
        /// <summary>
        /// Maliyet Daðýtým Fiþi
        /// </summary>
        public static TigerDataType CostDistributionSlips => new TigerDataType("costDistributionSlips");
        /// <summary>
        /// Malzeme Özellik Seti
        /// </summary>
        public static TigerDataType CharacteristicSets => new TigerDataType("characteristicSets");
        /// <summary>
        /// Malzeme Variantlarý
        /// </summary>
        public static TigerDataType Variants => new TigerDataType("variants");
        /// <summary>
        /// Muhasebe Baðlantý Kodlarý
        /// </summary>
        public static TigerDataType GLIntegrationCodes => new TigerDataType("glint?egrationCodes");
        /// <summary>
        /// Mühendislik deðiþiklikleri
        /// </summary>
        public static TigerDataType EngineeringChanges => new TigerDataType("engineeringChanges");
        /// <summary>
        /// Malzeme Kartý Kalite Kontrol Kriteri atamalarý
        /// </summary>
        public static TigerDataType QCCriteriaAssignments => new TigerDataType("qCCriteriaAssignments");
        /// <summary>
        /// Zimmet formu (Sabit Kýymet Ýþlemleri)
        /// </summary>
        public static TigerDataType FAAssignmentSlips => new TigerDataType("faassignmentSlips");
        /// <summary>
        /// Satýþ Teklif Formu
        /// </summary>
        public static TigerDataType SalesOffers => new TigerDataType("salesOffers");
        /// <summary>
        /// Satýþ Sözleþmesi
        /// </summary>
        public static TigerDataType SalesContracts => new TigerDataType("salesContracts");
        /// <summary>
        /// Satýþ Provizyon Daðýtým Fiþleri
        /// </summary>
        public static TigerDataType SalesProvisionDistributionSlips => new TigerDataType("salesProvisionDistributionSlips");
        /// <summary>
        /// Durma nedenleri
        /// </summary>
        public static TigerDataType StopCauses => new TigerDataType("StopCauses");
        /// <summary>
        ///	Satýþ Fýrsatlarý
        /// </summary>
        public static TigerDataType Opportunities => new TigerDataType("opportunities");
        /// <summary>
        ///	Satýþ Faaliyetleri
        /// </summary>
        public static TigerDataType SalesActivities => new TigerDataType("salesActivities");

    }

}