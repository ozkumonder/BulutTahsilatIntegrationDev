namespace BulutTahsilatIntegration.WinService.BTIIntegration
{
    public class TigerDataType
    {
        private TigerDataType(string value) { Value = value; }
        public string Value { get; set; }
        /// <summary>
        /// Malzeme Kart�
        /// </summary>
        public static TigerDataType Items => new TigerDataType("items"); 
        /// <summary>
        /// Malzeme Hareketleri
        /// </summary>
        public static TigerDataType ItemSlips => new TigerDataType("itemSlips");
        /// <summary>
        /// Al�nan Hizmet Kart�
        /// </summary>
        public static TigerDataType PurchasedServices => new TigerDataType("purchasedServices");
        /// <summary>
        /// Sat�� Sipari�i
        /// </summary>
        public static TigerDataType SalesOrders => new TigerDataType("salesOrders");
        /// <summary>
        /// Al�m Sipari�i
        /// </summary>
        public static TigerDataType PurchaseOrders => new TigerDataType("purchaseOrders");
        /// <summary>
        /// Al�m �ndirim Kart�
        /// </summary>
        public static TigerDataType PurchaseDiscounts => new TigerDataType("purchaseDiscounts");
        /// <summary>
        /// Al�m masraf kart�
        /// </summary>
        public static TigerDataType PurchaseExpenses => new TigerDataType("purchaseExpenses");
        /// <summary>
        /// Sat�� �ndirim Kart�
        /// </summary>
        public static TigerDataType SalesDiscounts => new TigerDataType("salesDiscounts");
        /// <summary>
        /// Sat�� Masraf Kart�
        /// </summary>
        public static TigerDataType SalesExpenses => new TigerDataType("salesExpenses");
        /// <summary>
        /// Al�m Promosyon
        /// </summary>
        public static TigerDataType PurchasePromotions => new TigerDataType("purchasePromotions");
        /// <summary>
        /// Sat�s Promosyon
        /// </summary>
        public static TigerDataType SalesPromotions => new TigerDataType("salesPromotions");
        /// <summary>
        /// Mal Al�m Fiyat Kart�
        /// </summary>
        public static TigerDataType PurchasedItemPrices => new TigerDataType("purchasedItemPrices");
        /// <summary>
        /// Hizmet Al�m Fiyat Kart�
        /// </summary>
        public static TigerDataType PurchasedServicePrices => new TigerDataType("purchasedServicePrices");
        /// <summary>
        /// Malzeme Sat�� Fiyat�
        /// </summary>
        public static TigerDataType SalesItemPrices => new TigerDataType("salesItemPrices");
        /// <summary>
        /// Hizmet Sat�� Fiyat�
        /// </summary>
        public static TigerDataType SalesServicePrices => new TigerDataType("salesServicePrices");
        /// <summary>
        /// Sat�c�-Cari Ba�lat�s�
        /// </summary>
        public static TigerDataType Salesmen => new TigerDataType("salesmen");
        /// <summary>
        /// Al�m �rsaliye
        /// </summary>
        public static TigerDataType PurchaseDispatches => new TigerDataType("purchaseDispatches");
        /// <summary>
        /// Sat�� �rsaliye
        /// </summary>
        public static TigerDataType SalesDispatches => new TigerDataType("salesDispatches");
        /// <summary>
        /// Al�m Fatura
        /// </summary>
        public static TigerDataType PurchaseInvoices => new TigerDataType("purchaseInvoices");
        /// <summary>
        ///  Sat�� Fatura
        /// </summary>
        public static TigerDataType SalesInvoices => new TigerDataType("salesInvoices");
        /// <summary>
        ///  �ek/Senet Devir
        /// </summary>
        public static TigerDataType ChequeAndPnotes => new TigerDataType("chequeAndPnotes");
        /// <summary>
        ///  �ek/Senet Bordorsu
        /// </summary>
        public static TigerDataType ChequeAndPnoteRolls => new TigerDataType("chequeAndPnoteRolls");
        /// <summary>
        ///  Banka Kart�
        /// </summary>
        public static TigerDataType Banks => new TigerDataType("banks");
        /// <summary>
        ///  Banka Hesap Kart�
        /// </summary>
        public static TigerDataType BankAccounts => new TigerDataType("bankAccounts");
        /// <summary>
        ///  Banka Fi�i
        /// </summary>
        public static TigerDataType BankSlips => new TigerDataType("bankSlips");
        /// <summary>
        ///  Muhasebe Hesap Kart�
        /// </summary>
        public static TigerDataType GLAccounts => new TigerDataType("gLAccounts");
        /// <summary>
        ///  Muhasebe Fi�i
        /// </summary>
        public static TigerDataType GLSlips => new TigerDataType("gLSlips");
        /// <summary>
        ///  Masraf Merkezi
        /// </summary>
        public static TigerDataType OverheadAccounts => new TigerDataType("overheadAccounts");
        /// <summary>
        ///  Kasa Kartlar�
        /// </summary>
        public static TigerDataType SafeDeposits => new TigerDataType("safeDeposits");
        /// <summary>
        ///  Kasa ��lemleri
        /// </summary>
        public static TigerDataType SafeDepositSlips => new TigerDataType("safeDepositSlips");
        /// <summary>
        ///  Cari Kart
        /// </summary>
        public static TigerDataType Arps => new TigerDataType("arps");
        /// <summary>
        ///  Cari Fi�
        /// </summary>
        public static TigerDataType ArpSlips => new TigerDataType("arpSlips");
        /// <summary>
        ///  �deme Plan�
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
        ///  Sabit K�ymet Kayd�
        /// </summary>
        public static TigerDataType FaRegistries => new TigerDataType("faRegistries");
        /// <summary>
        ///  Malzeme Farkl� Dilde A��klamalar
        /// </summary>
        public static TigerDataType ItemMLDescriptions => new TigerDataType("itemMLDescriptions");
        /// <summary>
        ///  Cari Kart Farkl� Dil A��klamalar�
        /// </summary>
        public static TigerDataType ArpMLDescriptons => new TigerDataType("ArpMLDescriptons");
        /// <summary>
        ///  Banka Kart� Farkl� Dilde A��klamalar
        /// </summary>
        public static TigerDataType BankMLDescriptions => new TigerDataType("bankMLDescriptions");
        /// <summary>
        /// Hesap Kart� Farkl� Dil A��klamalar
        /// </summary>
        public static TigerDataType GLAccountMLDescriptions => new TigerDataType("glAccountMLDescriptions");
        /// <summary>
        /// M��teri Yabanc� Dil A��klamalar�
        /// </summary>
        public static TigerDataType CustomerMLDescriptions => new TigerDataType("customerMLDescriptions");
        /// <summary>
        /// Alternatifler
        /// </summary>
        public static TigerDataType ItemAlternatives => new TigerDataType("	itemAlternatives");
        /// <summary>
        /// Malzeme Re�etesi
        /// </summary>
        public static TigerDataType ItemBoms => new TigerDataType("itemBoms");
        /// <summary>
        /// Seri Lot Tablosu
        /// </summary>
        public static TigerDataType SerialAndLotNumbers => new TigerDataType("serialAndLotNumbers");
        /// <summary>
        /// Malzeme �zellikleri
        /// </summary>
        public static TigerDataType ItemCharacteristics => new TigerDataType("itemCharacteristics");
        /// <summary>
        /// �� �stasyonu �zellikleri
        /// </summary>
        public static TigerDataType Characteristics => new TigerDataType("characteristics");
        /// <summary>
        /// �� �stasyonu
        /// </summary>
        public static TigerDataType Workstations => new TigerDataType("workstations");
        /// <summary>
        /// �� �stasyonu Grubu
        /// </summary>
        public static TigerDataType WorkstationGroups => new TigerDataType("workstationGroups");
        /// <summary>
        /// �al��an
        /// </summary>
        public static TigerDataType Employees => new TigerDataType("employees");
        /// <summary>
        /// �al��an Gruplar�
        /// </summary>
        public static TigerDataType EmployeeGroups => new TigerDataType("employeeGroups");
        /// <summary>
        /// �� �stasyonu Maliyetleri
        /// </summary>
        public static TigerDataType WorkstationCosts => new TigerDataType("workstationCosts");
        /// <summary>
        /// �al��an Maliyetleri
        /// </summary>
        public static TigerDataType EmployeeCosts => new TigerDataType("employeeCosts");
        /// <summary>
        /// Vardiya
        /// </summary>
        public static TigerDataType Shifts => new TigerDataType("shifts");
        /// <summary>
        /// Vardiya Atamas�
        /// </summary>
        public static TigerDataType ShiftAssignments => new TigerDataType("shiftAssignments");
        /// <summary>
        /// �r�n Re�etesi
        /// </summary>
        public static TigerDataType Boms => new TigerDataType("boms");
        /// <summary>
        /// Operasyonlar
        /// </summary>
        public static TigerDataType Operations => new TigerDataType("operations");
        /// <summary>
        /// Rota Tan�m�
        /// </summary>
        public static TigerDataType ProductionRoutes => new TigerDataType("productionRoutes");
        /// <summary>
        /// Re�ete Sabitleri
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
        /// Grup Kodlar�
        /// </summary>
        public static TigerDataType GroupCodes => new TigerDataType("groupCodes");
        /// <summary>
        /// Sat�c� Pozisyon Kodlar�
        /// </summary>
        public static TigerDataType SalesmanPositionCodes => new TigerDataType("salesmanPositionCodes");
        /// <summary>
        /// �deme Plan� Grup Kodu
        /// </summary>
        public static TigerDataType PaymentPlanGroupCodes => new TigerDataType("paymentPlanGroupCodes");
        /// <summary>
        /// �zel Kodlar
        /// </summary>
        public static TigerDataType SpecialCodes => new TigerDataType("specialCodes");
        /// <summary>
        /// Yetki Kodlar�
        /// </summary>
        public static TigerDataType AuthorizationCodes => new TigerDataType("authorizationCodes");
        /// <summary>
        /// Sat�c�-Cari Ba�lant�s�(Sat�� Y�netimi Flag�i A��kken)
        /// </summary>
        public static TigerDataType CustomersOfSalesmen => new TigerDataType("customersOfSalesmen");
        /// <summary>
        /// Sat�c� Rota Baglant�s�
        /// </summary>
        public static TigerDataType SalesmanRoutes => new TigerDataType("salesmanRoutes");
        /// <summary>
        /// Sat�c� Hedef Ba�lant�s�
        /// </summary>
        public static TigerDataType SalesmanDestinations => new TigerDataType("salesmanDestinations");
        /// <summary>
        /// Al�m Kampanyas�
        /// </summary>
        public static TigerDataType PurchaseCampaigns => new TigerDataType("purchaseCampaigns");
        /// <summary>
        /// Sat�� Kampanya
        /// </summary>
        public static TigerDataType SalesCampaigns => new TigerDataType("salesCampaigns");
        /// <summary>
        /// Da��t�m Arac�
        /// </summary>
        public static TigerDataType Vehicles => new TigerDataType("vehicles");
        /// <summary>
        /// Da��t�m Rotas�
        /// </summary>
        public static TigerDataType DistributionRoutes => new TigerDataType("distributionRoutes");
        /// <summary>
        /// Da��t�m Emri
        /// </summary>
        public static TigerDataType DistributionOrders => new TigerDataType("distributionOrders");
        /// <summary>
        /// �lke Bilgileri
        /// </summary>
        public static TigerDataType Countries => new TigerDataType("countries");
        /// <summary>
        /// �ehir Bilgileri
        /// </summary>
        public static TigerDataType Cities => new TigerDataType("cities");
        /// <summary>
        /// Posta Kodu Bilgisi
        /// </summary>
        public static TigerDataType PostCodes => new TigerDataType("postCodes");
        /// <summary>
        /// �l�e Bilgileri
        /// </summary>
        public static TigerDataType Towns => new TigerDataType("towns");
        /// <summary>
        /// Semt
        /// </summary>
        public static TigerDataType Districts => new TigerDataType("districts");
        /// <summary>
        /// Malzeme S�n�f� Atamas�
        /// </summary>
        public static TigerDataType ItemClassAssignments => new TigerDataType("itemClassAssignments");
        /// <summary>
        /// Standart Maliyet Periodlar�
        /// </summary>
        public static TigerDataType StandardCostPeriods => new TigerDataType("standardCostPeriods");
        /// <summary>
        /// Malzeme Standart Maliyeti
        /// </summary>
        public static TigerDataType ItemStandardCosts => new TigerDataType("itemStandardCosts");
        /// <summary>
        /// �� �stasyonu Standart Maliyeti
        /// </summary>
        public static TigerDataType WorkstationStandardCosts => new TigerDataType("workstationStandardCosts");
        /// <summary>
        /// �al��an Standart Maliyeti
        /// </summary>
        public static TigerDataType EmployeeStandardCosts => new TigerDataType("employeeStandardCosts");
        /// <summary>
        /// Standart Re�ete Maliyeti
        /// </summary>
        public static TigerDataType BomStandardCosts => new TigerDataType("bomStandardCosts");
        /// <summary>
        /// �� istasyonu �stisnalar�
        /// </summary>
        public static TigerDataType ProductionExceptions => new TigerDataType("productionExceptions");
        /// <summary>
        /// Verilen Hizmet Kart�
        /// </summary>
        public static TigerDataType SoldServices => new TigerDataType("soldServices");
        /// <summary>
        /// Ek Vergi Kart�
        /// </summary>
        public static TigerDataType AdditionalTaxes => new TigerDataType("additionalTaxes");
        /// <summary>
        /// �r�n Hatt�
        /// </summary>
        public static TigerDataType ProductionLines => new TigerDataType("productionLines");
        /// <summary>
        /// Talep Kar��lama
        /// </summary>
        public static TigerDataType DemandPeggings => new TigerDataType("demandPeggings");
        /// <summary>
        /// Fiyat Fark� Faturas�
        /// </summary>
        public static TigerDataType PaymentDifferenceInvoices => new TigerDataType("paymentDifferenceInvoices");
        /// <summary>
        /// Proje Kart�
        /// </summary>
        public static TigerDataType Projects => new TigerDataType("projects");
        /// <summary>
        /// Geri �deme Planlar�
        /// </summary>
        public static TigerDataType RepaymentPlans => new TigerDataType("repaymentPlans");
        /// <summary>
        /// Da��t�m �ablonlar�
        /// </summary>
        public static TigerDataType DistributionTemplates => new TigerDataType("distributionTemplates");
        /// <summary>
        /// Stok Yeri Kodlar�
        /// </summary>
        public static TigerDataType LocationCodes => new TigerDataType("locationCodes");
        /// <summary>
        /// Sat�� Ko�ullar� (Fi� Sat�rlar�)
        /// </summary>
        public static TigerDataType SalesConditionsForSlipLines => new TigerDataType("salesConditionsForSlipLines");
        /// <summary>
        /// Sat�� Ko�ullar� (Fi� Geneli)
        /// </summary>
        public static TigerDataType SalesConditionsForSlips => new TigerDataType("salesConditionsForSlips");
        /// <summary>
        /// Al�� Ko�ullar� (Fi� Sat�rlar�)
        /// </summary>
        public static TigerDataType PurchaseConditionsForSlipLines => new TigerDataType("purchaseConditionsForSlipLines");
        /// <summary>
        /// Al�� Ko�ullar� (Fi� Geneli) 
        /// </summary>
        public static TigerDataType PurchaseConditionsForSlips => new TigerDataType("purchaseConditionsForSlips");
        /// <summary>
        /// Talep
        /// </summary>
        public static TigerDataType DemandSlips => new TigerDataType("demandSlips");
        /// <summary>
        /// �hracat Kredisi (D�viz/Exim)
        /// </summary>
        public static TigerDataType ExportCredits => new TigerDataType("exportCredits");
        /// <summary>
        /// Serbest B�lge Tan�m�
        /// </summary>
        public static TigerDataType FreeZones => new TigerDataType("freeZones");
        /// <summary>
        /// G�mr�k Tan�m�
        /// </summary>
        public static TigerDataType CustomsOffices => new TigerDataType("customsOffices");
        /// <summary>
        /// �thalat Operasyon Fi�i
        /// </summary>
        public static TigerDataType ImportOperationSlips => new TigerDataType("importOperationSlips");
        /// <summary>
        /// �hracat Operasyon Fi�i
        /// </summary>
        public static TigerDataType ExportOperationSlips => new TigerDataType("exportOperationSlips");
        /// <summary>
        /// �hracat / �hra� Kay�tl� Al�m Faturalar�
        /// </summary>
        public static TigerDataType ExportTypedPurchaseInvoices => new TigerDataType("exportTypedPurchaseInvoices");
        /// <summary>
        /// �hracat / �hra� Kay�tl� Sat�� Faturalar�
        /// </summary>
        public static TigerDataType ExportTypedSalesInvoices => new TigerDataType("exportTypedSalesInvoices");
        /// <summary>
        /// �hracat / Dahilde ��leme �zin Belgesi
        /// </summary>
        public static TigerDataType InwardProcessingPermits => new TigerDataType("inwardProcessingPermits");
        /// <summary>
        /// �thalat / Malzeme Dola��m Fi�i
        /// </summary>
        public static TigerDataType ExportMovementSlips => new TigerDataType("exportMovementSlips");
        /// <summary>
        /// �thalat / Millile�tirme Fi�leri
        /// </summary>
        public static TigerDataType ExportNationalizationSlips => new TigerDataType("exportNationalizationSlips");
        /// <summary>
        /// �thalat / Da��t�m Fi�leri
        /// </summary>
        public static TigerDataType ImportDistributionSlips => new TigerDataType("importDistributionSlips");
        /// <summary>
        /// Marka Tan�mlar�
        /// </summary>
        public static TigerDataType ItemBrands => new TigerDataType("itemBrands");
        /// <summary>
        /// Tan�ml� Alanlar
        /// </summary>
        public static TigerDataType ExtendedFields => new TigerDataType("extendedFields");
        /// <summary>
        /// Tan�ml� Alan Tan�mlar�
        /// </summary>
        public static TigerDataType ExtendedFieldDefinitions => new TigerDataType("extendedFieldDefinitions");
        /// <summary>
        /// Zorunlu Alanlar
        /// </summary>
        public static TigerDataType MandatoryFields => new TigerDataType("mandatoryFields");
        /// <summary>
        /// Tan�ml� Alan Kategori Listeleri
        /// </summary>
        public static TigerDataType ExtendedFieldCategories => new TigerDataType("extendedFieldCategories");
        /// <summary>
        /// �� Ak�� Y�netimi  / AnaKay�tlar / �� Ak�� Rol Tan�mlar�
        /// </summary>
        public static TigerDataType WorkflowRoles => new TigerDataType("workflowRoles");
        /// <summary>
        /// �� Ak�� Y�netimi  / AnaKay�tlar / �� Ak�� Kartlar�
        /// </summary>
        public static TigerDataType WorkflowDefinitions => new TigerDataType("workflowDefinitions");
        /// <summary>
        /// Planlanan/Ger�ekle�en Kaynak Kullan�m� Giri�i
        /// </summary>
        public static TigerDataType ProductionResourceUtilization => new TigerDataType("productionResourceUtilization");
        /// <summary>
        /// Grup �irketi
        /// </summary>
        public static TigerDataType ArpGroupAssignments => new TigerDataType("arpGroupAssignments");
        /// <summary>
        /// Teminat Bordrolar�
        /// </summary>
        public static TigerDataType CollateralRolls => new TigerDataType("collateralRolls");
        /// <summary>
        /// Sat�nalma Teklif Y�netimi � Emir
        /// </summary>
        public static TigerDataType PurchaseProposalOrders => new TigerDataType("purchaseProposalOrders");
        /// <summary>
        /// Sat�nalma Teklif Y�netimi � Teklif
        /// </summary>
        public static TigerDataType PurchaseProposalOffers => new TigerDataType("purchaseProposalOffers");
        /// <summary>
        /// Sat�nalma Teklif Y�netimi � S�zle�me
        /// </summary>
        public static TigerDataType PurchaseProposalContracts => new TigerDataType("purchaseProposalContracts");
        /// <summary>
        /// H�zl� �retim Fi�i
        /// </summary>
        public static TigerDataType QuickProductionSlips => new TigerDataType("quickProductionSlips");
        /// <summary>
        /// M��teriler (Teklif Y�netimi Kategorileri)
        /// </summary>
        public static TigerDataType Customers => new TigerDataType("customers");
        /// <summary>
        /// Sat�� Kategorileri
        /// </summary>
        public static TigerDataType SalesCategories => new TigerDataType("salesCategories");
        /// <summary>
        /// �lgili Ki�iler (Teklif Y�netimi Kategorileri)
        /// </summary>
        public static TigerDataType Contacts => new TigerDataType("contacts");
        /// <summary>
        /// Banka Kredileri
        /// </summary>
        public static TigerDataType BankCredits => new TigerDataType("bankCredits");
        /// <summary>
        /// Maliyet Da��t�m Fi�i
        /// </summary>
        public static TigerDataType CostDistributionSlips => new TigerDataType("costDistributionSlips");
        /// <summary>
        /// Malzeme �zellik Seti
        /// </summary>
        public static TigerDataType CharacteristicSets => new TigerDataType("characteristicSets");
        /// <summary>
        /// Malzeme Variantlar�
        /// </summary>
        public static TigerDataType Variants => new TigerDataType("variants");
        /// <summary>
        /// Muhasebe Ba�lant� Kodlar�
        /// </summary>
        public static TigerDataType GLIntegrationCodes => new TigerDataType("glint?egrationCodes");
        /// <summary>
        /// M�hendislik de�i�iklikleri
        /// </summary>
        public static TigerDataType EngineeringChanges => new TigerDataType("engineeringChanges");
        /// <summary>
        /// Malzeme Kart� Kalite Kontrol Kriteri atamalar�
        /// </summary>
        public static TigerDataType QCCriteriaAssignments => new TigerDataType("qCCriteriaAssignments");
        /// <summary>
        /// Zimmet formu (Sabit K�ymet ��lemleri)
        /// </summary>
        public static TigerDataType FAAssignmentSlips => new TigerDataType("faassignmentSlips");
        /// <summary>
        /// Sat�� Teklif Formu
        /// </summary>
        public static TigerDataType SalesOffers => new TigerDataType("salesOffers");
        /// <summary>
        /// Sat�� S�zle�mesi
        /// </summary>
        public static TigerDataType SalesContracts => new TigerDataType("salesContracts");
        /// <summary>
        /// Sat�� Provizyon Da��t�m Fi�leri
        /// </summary>
        public static TigerDataType SalesProvisionDistributionSlips => new TigerDataType("salesProvisionDistributionSlips");
        /// <summary>
        /// Durma nedenleri
        /// </summary>
        public static TigerDataType StopCauses => new TigerDataType("StopCauses");
        /// <summary>
        ///	Sat�� F�rsatlar�
        /// </summary>
        public static TigerDataType Opportunities => new TigerDataType("opportunities");
        /// <summary>
        ///	Sat�� Faaliyetleri
        /// </summary>
        public static TigerDataType SalesActivities => new TigerDataType("salesActivities");

    }

}