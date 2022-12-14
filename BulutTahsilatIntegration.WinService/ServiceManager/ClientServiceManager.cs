using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BulutTahsilatIntegration.WinService.BTIIntegration;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.DataAccess;
using BulutTahsilatIntegration.WinService.IntegrationService;
using BulutTahsilatIntegration.WinService.Model;
using BulutTahsilatIntegration.WinService.Model.Dtos;
using BulutTahsilatIntegration.WinService.Model.Enums;
using BulutTahsilatintegration.WinService.Model.ErpModel;
using BulutTahsilatIntegration.WinService.Model.ErpModel;
using BulutTahsilatIntegration.WinService.Model.Global;
using BulutTahsilatIntegration.WinService.Model.Logging;
using BulutTahsilatIntegration.WinService.Utilities;
using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.ServiceManager
{
    public static class ClientServiceManager
    {
        const string apiCode = "";
        const string apiUser = "";
        const string apiPass = "";
        static WSBankPaymentServiceSoapClient serviceClient = new WSBankPaymentServiceSoapClient("WSBankPaymentServiceSoap");
        private static readonly RestServiceManager restServiceManager = new RestServiceManager();
        private static DataAccessException exception;
        private static int FirmReportCurrencyType = DataLogic.GetReportCurrencyType(GlobalSettings.LogoFirmNumber.ToInt());

        #region Banka Bakiyeleri

        public static void GetBankBalance()
        {
            try
            {

                var bankbalances = serviceClient.GetFirmBankBalance(apiUser, apiPass, apiCode, string.Empty);
                var balances = new List<BankBalances>();
                foreach (var balance in bankbalances.BankList)
                {
                    balances.Add(new BankBalances
                    {
                        FIRMNAME = balance.FirmName,
                        BANKCODE = balance.FirmBankCode,
                        BANKNAME = balance.FirmBankName,
                        BANKBRANCHNAME = balance.FirmBankBranchName,
                        BANKNICKNAME = balance.FirmBankNickName,
                        BANKACCOUNTTYPE = balance.FirmBankAccountType,
                        BANKIBAN = balance.FirmBankIBAN,
                        BANKCURRENCYUNIT = balance.FirmBankCurrencyUnit,
                        LASTTIMESTMAP = balance.LastTimeStamp,
                        BALANCE = balance.Balance,
                        BLOCKEDBALANCE = balance.BlockedBalance
                    });
                }

                var serviceResult = DataLogic.InsertBankBalance(balances);
                if (!serviceResult.Success)
                {
                    LogHelper.LogError(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                        typeof(DataLogic), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name, StringUtil.Seperator, serviceResult.ErrorDesc));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        #endregion

        #region Cari İşlemleri
        public static void SendClient()
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Cari Hesap İşlemleri Başlangıç------------->"));
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Cari listesi okunuyor..."));
            var clients = DataLogic.GetClients();
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, clients.Count, " cari hesap kaydı bulundu..."));
            foreach (var client in clients)
            {
                var error = new TransferErrorLog();
                error = new TransferErrorLog
                {
                    PAYMENTID = 99,
                    FICHEDATE = DateTime.Now,
                    BULUTFICHETYPE = "Cari Gönderimi",
                    LOGOFICHETYPE = "Cari Gönderimi",
                };
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, client.ClientCode, " kodlu cari hesap bulut portal'a gönderiliyor..."));
                var clientResponse = serviceClient.SubFirmAddNew(apiUser, apiPass, apiCode, new SubFirm
                {
                    FirmName = client.FirmName,
                    Address = client.Address,
                    County = client.Country,
                    CityID = client.CityID.ToInt(),
                    TaxNumber = client.TaxNumber,
                    TaxOffice = client.TaxOffice,
                    AccountingCode = client.ClientCode,//todo client.AccountingCode yerine client.ClientCode olarak değiştirildi 10_09_2021
                    Status = EnumStatus.Active
                });
                if (clientResponse.StatusMessage == "Success" || clientResponse.StatusMessage.Contains("VKN_VE_CARI_UNVANI_BASKA_BIR_CARIDE_MEVCUT"))
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, client.ClientCode, " kodlu cari hesap bulut portala aktarılmıştır."));
                    error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", client.ClientCode, " kodlu cari hesap bulut portala aktarılmıştır.");
                    error.PAYMENTEXPCODE = clientResponse.SubFirmReturn.PaymentExpCode;
                    error.ERPRESPONSE = clientResponse.StatusMessage;
                    var result = DataLogic.UpdateClientEdiNo(clientResponse.SubFirmReturn.FirmCode, client.Id);
                    if (result)
                    {
                        var clientPairInfo = DataLogic.GetClientPairInfoByPaymetExpCode(clientResponse.SubFirmReturn.PaymentExpCode);

                        var ibans = DataLogic.GetClientIbansByTaxNumber(client.TaxNumber);
                        foreach (var iban in ibans)
                        {
                            var ibanResponse = ClientIbanAdd(clientResponse.SubFirmReturn.PaymentExpCode, iban.Iban, iban.BankAccountCode);
                        }
                        if (!string.IsNullOrEmpty(clientPairInfo.TAXNR))
                        {
                            var vknResponse = ClientTaxNumberAdd(clientResponse.SubFirmReturn.PaymentExpCode, client.TaxNumber, clientPairInfo.NAME, clientPairInfo.SURNAME);
                        }
                    }
                }
                else
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, client.ClientCode, " kodlu cari hesap gönderimi sırasında hata oluştu... ", clientResponse.StatusMessage));
                    error.ERROR = string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator,
                        client.ClientCode, " kodlu cari hesap gönderimi sırasında hata oluştu... ",
                        clientResponse.StatusMessage);
                    var result = DataLogic.UpdateClientEdiNo(clientResponse.StatusMessage, client.Id);
                }
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Cari Hesap İşlemleri Bitiş------------->", Environment.NewLine));
        }
        public static ResponseSubFirmIBAN ClientIbanAdd(string paymentExpCode, string iban, string bankCode)
        {
            var clientIbanRespose = serviceClient.SubFirmIBANAddNew(apiUser, apiPass, apiCode, paymentExpCode, iban, bankCode);
            return clientIbanRespose;
        }
        public static ResponseSubFirmVKN ClientTaxNumberAdd(string paymentExpCode, string vkn, string name, string surName)
        {
            var clientVknRespose = serviceClient.SubFirmVKNAddNew(apiUser, apiPass, apiCode, paymentExpCode, vkn, name, surName);
            return clientVknRespose;
        }
        public static void SendServiceCard()
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Cari Hesap İşlemleri Başlangıç------------->"));
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Hizmet kartı listesi okunuyor..."));
            var clients = DataLogic.GetServiceCards();
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, clients.Count, " hizmet kaydı bulundu..."));
            foreach (var client in clients)
            {
                var error = new TransferErrorLog();
                error = new TransferErrorLog
                {
                    PAYMENTID = 99,
                    FICHEDATE = DateTime.Now,
                    BULUTFICHETYPE = "Hizmet Kartı Gönderimi",
                    LOGOFICHETYPE = "Hizmet Kartı Gönderimi",
                };
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, client.ClientCode, " kodlu hizmet kartı bulut portal'a gönderiliyor..."));
                var clientResponse = serviceClient.SubFirmAddNew(apiUser, apiPass, apiCode, new SubFirm
                {
                    FirmName = client.FirmName,
                    Address = client.Address,
                    County = client.Country,
                    CityID = client.CityID.ToInt(),
                    TaxNumber = client.TaxNumber,
                    TaxOffice = client.TaxOffice,
                    AccountingCode = client.AccountingCode,
                    Status = EnumStatus.Active
                });
                if (clientResponse.StatusMessage == "Success" || clientResponse.StatusMessage.Contains("VKN_VE_CARI_UNVANI_BASKA_BIR_CARIDE_MEVCUT"))
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, client.ClientCode, " kodlu hizmet kartı bulut portala aktarılmıştır..."));
                    error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", client.ClientCode, " kodlu hizmet kartı bulut portala aktarılmıştır...");
                    error.PAYMENTEXPCODE = clientResponse.SubFirmReturn.PaymentExpCode;
                    error.ERPRESPONSE = clientResponse.StatusMessage;
                    var clientResponseJson = JsonConvert.SerializeObject(clientResponse.StatusMessage, Formatting.Indented,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    LogHelper.Log(clientResponseJson);
                    var result = DataLogic.UpdateSrvCardDef2(clientResponse.SubFirmReturn.FirmCode, client.Id);
                }
                else
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, client.ClientCode, " kodlu hizmet kartı gönderimi sırasında hata oluştu...", clientResponse.StatusMessage));
                    error.ERROR = string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator,
                        client.ClientCode, " kodlu hizmet kartı gönderimi sırasında hata oluştu...",
                        clientResponse.StatusMessage);
                    var result = DataLogic.UpdateSrvCardDef2(clientResponse.StatusMessage, client.Id);
                }
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }
        }

        #endregion

        #region Bulut Tahsilat İşlemleri Post - Get

        #region Bulut Transaction Durum Güncelleme
        public static string UpdatePaymentStatusInfo(int paymentId, BankPaymentListItem paymentListItem)
        {
            var error = new TransferErrorLog();
            var result = string.Empty;

            try
            {
                error = new TransferErrorLog
                {
                    PAYMENTID = paymentListItem.PaymentID,
                    FICHEDATE = paymentListItem.PaymentDate,
                    PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                    BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                    LOGOFICHETYPE = "UpdatePaymentStatusInfo",
                    SENDERFIRMNAME = paymentListItem.SenderFirmName,
                    SENDERBANK = paymentListItem.SenderFirmBankName,
                    SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                    BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                    FIRMBANK = paymentListItem.FirmBankName,
                    FIRMIBAN = paymentListItem.FirmBankIBAN,
                    EXPLANATION = paymentListItem.Explanation,

                };
                result = serviceClient.UpdatePaymentStatusInfo(apiUser, apiPass, apiCode, paymentId,
                    (int)PaymentStatusTypeID.Tamamlandi);
                error.STATUSINFO = result;

            }
            catch (Exception e)
            {
                error.ERROR = e.Message;
            }
            finally
            {
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
                if (!serviceResult.Success)
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                        nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                        StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!", exception.ErrorDesc, " - ", error.ERROR));
                }
            }
            return result;
        }
        #endregion

        #region Gelen Havale İşlemi

        /// <summary>
        /// Parametre verilen tarih arasındaki gelen havale fişlerini getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetIncommingTransferOnBulut(DateTime begDate, DateTime endDate, string taxNumber)
        {
            var result = serviceClient.BankPaymentList(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && !string.IsNullOrEmpty(x.FirmBankIBAN) && !string.IsNullOrEmpty(x.PaymentExpCode) && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Gelen Havale",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetIncommingTransferOnBulut/BankPaymentList",
                Filters = "Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && !string.IsNullOrEmpty(x.FirmBankIBAN) && !string.IsNullOrEmpty(x.PaymentExpCode)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }

            return filtredData;
        }

        #endregion

        #region Giden Havale İşlemi

        private static List<BankPaymentListItem> GetSendingTransferOnBulut(DateTime begDate, DateTime endDate, string taxNumber)
        {
            var result = serviceClient.BankPaymentListDebit(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.BranchFirmTaxNumber == taxNumber).ToList();

            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Giden Havale",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetSendingTransferOnBulut/BankPaymentListDebit",
                Filters = "Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }

            return filtredData;
        }
        #endregion

        #region Virman İşlemi

        /// <summary>
        /// Bulut tahsilat üzerinde işlem tipi virman olan karşı banka iban bilgisi dolu olan ve PN419419843250 kodlu kayıtları getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetBankTransferOnBulut(DateTime begDate, DateTime endDate, string taxNumber)
        {
            var result = serviceClient.BankPaymentListDebit(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.Virman && !string.IsNullOrEmpty(x.SenderFirmBankIBAN) && x.PaymentExpCode == "PN589524506824" && x.BranchFirmTaxNumber == taxNumber).ToList();//146171196 145367628
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Virman Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetBankTransferOnBulut/BankPaymentListDebit",
                Filters = @"Where(x => x.PaymentTypeID == (int)PaymentTypeID.Virman && !string.IsNullOrEmpty(x.SenderFirmBankIBAN) && x.PaymentExpCode == PN419419843250)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return filtredData;
        }
        #endregion

        #region Döviz Alım İşlemi

        /// <summary>
        /// Bulut tahsilat üzerindeki döviz alım işlemlerini listeler
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetBuyExchangeBankTransferOnBulut(DateTime begDate, DateTime endDate,string taxNumber)
        {
            var result = serviceClient.BankPaymentList(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);

            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.SenderFirmName == "DOVIZ ALIM" && string.IsNullOrEmpty(x.CustomField2) == false && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Döviz Alım Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetExchangeBankTransferOnBulut/BankPaymentList",
                Filters = @"result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.SenderFirmName.Equals(ARBİTRAJ) && string.IsNullOrEmpty(x.CustomField2) == false).ToList()",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return filtredData;
            //return result.Where(x => x.PaymentID == 133210872).ToList();
        }

        /// <summary>
        /// Parametre verilen dekont numarası için döviz alım borç hareketini getirir
        /// </summary>
        /// <param name="date"></param>
        /// <param name="customField"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static BankPaymentListItem GetBuyExchangeCreditBankTransferOnBulut(DateTime date, string customField, string taxNumber)
        {
            var begDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            var result = serviceClient.BankPaymentListAll(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var debit = result.FirstOrDefault(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.CustomField2 == customField && x.SenderFirmName == "DOVIZ ALIM" && x.Amount < 0 && x.AccountCurrencyCode == "TRY" && x.BranchFirmTaxNumber == taxNumber);
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Döviz Alım Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetExchangeCreditBankTransferOnBulut/BankPaymentListAll",
                Filters = @"result.FirstOrDefault(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.CustomField2 == customField && x.SenderFirmName.Equals(ARBİTRAJ) && x.Amount < 0 && x.AccountCurrencyCode == TRY)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(debit)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return debit;
            //return result.Where(x => x.PaymentID == 133210904).FirstOrDefault();
        }

        private static List<BankPaymentListItem> GetSalesExchangeBankTransferOnBulut(DateTime begDate, DateTime endDate,string taxNumber)
        {
            var result = serviceClient.BankPaymentList(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);

            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.SenderFirmName == "DOVIZ ALIM" && string.IsNullOrEmpty(x.CustomField2) == false && x.AccountCurrencyCode == "TRY" && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Döviz Satış Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetExchangeBankTransferOnBulut/BankPaymentList",
                Filters = @"result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.SenderFirmName.Equals(ARBİTRAJ) && string.IsNullOrEmpty(x.CustomField2) == false).ToList()",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return filtredData;
            //return result.Where(x => x.PaymentID == 133210872).ToList();
        }

        /// <summary>
        /// Parametre verilen dekont numarası için döviz alım borç hareketini getirir
        /// </summary>
        /// <param name="date"></param>
        /// <param name="customField"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static BankPaymentListItem GetSalesExchangeCreditBankTransferOnBulut(DateTime date, string customField, string taxNumber)
        {
            var begDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            var result = serviceClient.BankPaymentListAll(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var debit = result.FirstOrDefault(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.CustomField2 == customField && x.SenderFirmName == "DOVIZ ALIM" && x.Amount < 0 && x.BranchFirmTaxNumber == taxNumber);
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Döviz Satış Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetExchangeCreditBankTransferOnBulut/BankPaymentListAll",
                Filters = @"result.FirstOrDefault(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.CustomField2 == customField && x.SenderFirmName.Equals(ARBİTRAJ) && x.Amount < 0 && x.AccountCurrencyCode == TRY)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(debit)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return debit;
            //return result.Where(x => x.PaymentID == 133210904).FirstOrDefault();
        }

        #endregion

        #region Arbitraj İşlemi

        /// <summary>
        /// Bulut tahsilat üzerinde işlem tipi virman olan açıklamasında arbitraj geçen kayıtları getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetArbitajTransferOnBulut(DateTime begDate, DateTime endDate,string taxNumber)
        {
            var result = serviceClient.BankPaymentList(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && string.IsNullOrEmpty(x.CustomField2) == false && x.SenderFirmName.Equals("ARBİTRAJ") && x.AccountCurrencyCode != "TRY" && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Arbitraj Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetArbitajTransferOnBulut/BankPaymentList",
                Filters = @"result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && string.IsNullOrEmpty(x.CustomField2) == false && x.SenderFirmName.Equals(ARBİTRAJ)).ToList()",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return filtredData;
            //return result.Where(x => x.PaymentID == 128894614).ToList();
        }

        private static BankPaymentListItem GetArbitrajCreditBankTransferOnBulut(DateTime date, string customField, string taxNumber)
        {
            var begDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            var result = serviceClient.BankPaymentListAll(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var debit = result.FirstOrDefault(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.CustomField2 == customField && x.SenderFirmName.Equals("ARBİTRAJ") && x.Amount < 0 && x.AccountCurrencyCode != "TRY" && x.BranchFirmTaxNumber == taxNumber);

            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Arbitraj Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetArbitrajCreditBankTransferOnBulut/BankPaymentListAll",
                Filters = @"result.FirstOrDefault(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.CustomField2 == customField && x.SenderFirmName.Equals(ARBİTRAJ) && x.Amount < 0 && x.AccountCurrencyCode != TRY)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(debit)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }
            return debit;
            //return result.Where(x => x.PaymentID == 128901008).FirstOrDefault();
        }

        #endregion

        #region Çek İşlemi
        /// <summary>
        /// Çek kayıtlarını getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetCheckTransferOnBulut(DateTime begDate, DateTime endDate,string taxNumber)
        {
            var result = serviceClient.BankPaymentListAll(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.Cek && !string.IsNullOrEmpty(x.CheckNumber) && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Çek Hareketi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetCheckTransferOnBulut/BankPaymentListDebit",
                Filters = @"Where(x => x.PaymentTypeID == (int)PaymentTypeID.Cek && !string.IsNullOrEmpty(x.CheckNumber))",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }

            return filtredData;

        }

        #endregion

        #region Banka Hizmet Faturası İşlemi
        /// <summary>
        /// Banka hzimet faturalarını getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetBankServiceInvoiceTransferOnBulut(DateTime begDate, DateTime endDate, string taxNumber)
        {
            var result = serviceClient.BankPaymentListAll(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.Masraf || x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Banka Hizmet Faturası",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetBankServiceInvoiceTransferOnBulut/BankPaymentListAll",
                Filters = @"Where(x => x.PaymentTypeID == (int)PaymentTypeID.Masraf || x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }

            return filtredData;
        }

        #endregion

        #region Leasing Taksit Ödeme İşlemi
        /// <summary>
        /// Leasing taksit hareketini getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetLeasingPaySendingTransferOnBulut(DateTime begDate, DateTime endDate, string taxNumber)
        {
            var result = serviceClient.BankPaymentListDebit(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi && !string.IsNullOrEmpty(x.CustomField1) && x.BranchFirmTaxNumber == taxNumber).ToList();

            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Leasing Taksidi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetLeasingPaySendingTransferOnBulut/BankPaymentListDebit",
                Filters = "Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }

            return filtredData;
        }


        #endregion
        #region Kredi Taksit Ödeme İşlemi
        /// <summary>
        /// Kredi taksit hareketini getirir.
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        private static List<BankPaymentListItem> GetCreaditPaySendingTransferOnBulut(DateTime begDate, DateTime endDate, string taxNumber)
        {
            var result = serviceClient.BankPaymentListDebit(apiUser, apiPass, apiCode, (int)PaymentStatusTypeID.EslesmeYapildi, begDate, endDate).OrderByDescending(x => x.PaymentDate);
            //var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.Kredi && !string.IsNullOrEmpty(x.CustomField2)).ToList();
            var filtredData = result.Where(x => x.PaymentTypeID == (int)PaymentTypeID.Kredi && x.BranchFirmTaxNumber == taxNumber).ToList();
            var getId = DataLogic.InsertGetLog(new GetLog
            {
                ProcDate = DateTime.Now,
                OperationName = "Kredi Taksidi",
                PaymentStatusTypeId = (int)PaymentStatusTypeID.EslesmeYapildi,
                BegDate = begDate,
                EndDate = endDate,
                MethodName = "GetCreaditPaySendingTransferOnBulut/BankPaymentListDebit",
                Filters = "Where(x => x.PaymentTypeID == (int)PaymentTypeID.BankaHareketi)",
                RawData = JsonConvert.SerializeObject(result),
                FilteredData = JsonConvert.SerializeObject(filtredData)
            }, ref exception);
            if (!getId.Success)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, "InsertGetLog kaydı oluşturululamadı!", StringUtil.Seperator, exception.Message));
            }

            return filtredData;
        }


        #endregion

        #endregion

        #region Logo Transfer İşlemleri
        /// <summary>
        /// Gelen Havale
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static void IncommingTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Gelen Havale İşlemleri Başlangıç------------->"));
            BankPaymentListItem credit = null;
            var error = new TransferErrorLog();
            var paymentId = string.Empty;
            decimal curr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Gelen havale kayıtları sorgulanıyor..."));
                var paymentListItems = GetIncommingTransferOnBulut(begDate, endDate, taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet gelen havale kaydı bulundu...") : "Gelen havale kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Gelen Havale",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                    curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                    var clientInfo = DataLogic.GetClientCodeByPNCode(paymentListItem.PaymentExpCode);
                    var bankAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (!string.IsNullOrEmpty(clientInfo) && !string.IsNullOrEmpty(bankAccountInfo.BankAccountCode))
                    {
                        var bankSlip = new BankSlip
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = paymentListItem.Amount < 0 ? 4 : 3,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };

                        var item = new Transaction
                        {

                            Type = 1,
                            Tranno = "~",
                            BankRef = bankAccountInfo.BankRef,
                            BankaccCode = bankAccountInfo.BankAccountCode,
                            ArpCode = DataLogic.GetClientCodeByPNCode(paymentListItem.PaymentExpCode),
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Debit = paymentListItem.Amount,
                            Amount = paymentListItem.Amount,
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = paymentListItem.Amount,
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = paymentListItem.Amount,
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BankProcType = 2,
                            BnCrdtype = bankAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankAccountInfo.BankAccountCode),
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };
                        bankSlip.Transactions.items.Add(item);
                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);
                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu gelen havale ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }
                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!", paymentListItem.PaymentID, Environment.NewLine, paymentJson));
                        error.ERROR = string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!");
                        if (clientInfo == null)
                        {
                            var message = string.Concat(paymentListItem.PaymentExpCode,
                                " nolu PN koduna ait cari bilgisi bulunamamıştır! ", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }

                        if (bankAccountInfo.BankAccountCode == null)
                        {
                            var message = string.Concat(
                                paymentListItem.PaymentExpCode, StringUtil.Seperator,
                                paymentListItem.FirmBankIBAN,
                                " nolu PN kodlu ve Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!",
                                paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                    }
                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Gelen Havale İşlemleri Bitiş------------->"));
        }
        /// <summary>
        /// Gönderilen Havale
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static void SendingTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Giden Havale İşlemleri Başlangıç------------->"));
            BankPaymentListItem credit = null;
            var error = new TransferErrorLog();
            decimal curr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Giden havale kayıtları sorgulanıyor..."));
                var paymentListItems = GetSendingTransferOnBulut(begDate, endDate, taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet giden havale kaydı bulundu...") : "Giden havale kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Gönderilen Havale",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                    curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                    var client = DataLogic.GetClientCodeByPNCode(paymentListItem.PaymentExpCode);
                    var bankAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (!string.IsNullOrEmpty(client) && !string.IsNullOrEmpty(bankAccountInfo.BankAccountCode))
                    {
                        var bankSlip = new BankSlip
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = paymentListItem.Amount < 0 ? 4 : 3,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var item = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankAccountInfo.BankRef,
                            BankaccCode = bankAccountInfo.BankAccountCode,
                            ArpCode = client,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Credit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BankProcType = 2,
                            BnCrdtype = bankAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankAccountInfo.BankAccountCode),
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };
                        bankSlip.Transactions.items.Add(item);
                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);

                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu gönderilen havale ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;

                            // todo ilgili tamamlandı olarak geri gönder
                        }

                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!", paymentListItem.PaymentID, Environment.NewLine, paymentJson));
                        error.ERROR = string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!");
                        if (client == null)
                        {
                            var message = string.Concat(paymentListItem.PaymentExpCode,
                                " nolu PN koduna ait cari bilgisi bulunamamıştır! ", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                        if (string.IsNullOrEmpty(bankAccountInfo.Iban))
                        {
                            var message = string.Concat(paymentListItem.PaymentExpCode, StringUtil.Seperator,
                                paymentListItem.FirmBankIBAN,
                                " nolu PN kodlu ve Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                        }
                    }
                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Giden Havale İşlemleri Bitiş------------->"));
        }
        /// <summary>
        /// Virman 
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        public static void BankTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Virman İşlemleri Başlangıç------------->"));
            var error = new TransferErrorLog();
            decimal curr = 0;
            decimal debitCurr = 1;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Virman kayıtları sorgulanıyor..."));
                var paymentListItems = GetBankTransferOnBulut(begDate, endDate, taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet virman kaydı bulundu...") : "Virman kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Bank Virman Fişi",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var bankCreditAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    var bankDebitAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.SenderFirmBankIBAN);


                    if (bankCreditAccountInfo.BankAccountCode != null && bankDebitAccountInfo.BankAccountCode != null)
                    {
                        if (paymentListItem.AccountCurrencyCode != "TRY" || bankDebitAccountInfo.CurrencyCode != "TL" || bankDebitAccountInfo.CurrencyCode != "TL")
                        {
                            var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                            var debitCurrencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == bankDebitAccountInfo.CurrencyCode);
                            curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                            debitCurr = DataLogic.GetDailyExchange(debitCurrencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();

                            if (bankDebitAccountInfo.CurrencyCode == "TL" && bankCreditAccountInfo.CurrencyCode != "TL")
                            {
                                debitCurr = curr;
                            }
                            if (curr == 1 && debitCurr == 1)
                            {
                                var message = string.Concat(paymentListItem.PaymentID,
                                    " işlem kodlu ödeme kaydına istinaden logoda günlük döviz kuru bulunamadı!");
                                UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                                error.ERROR = message;
                                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), string.Concat(nameof(ClientServiceManager), " ", MethodBase.GetCurrentMethod().Name, " ", message)));
                                var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                                if (!insertReslut.Success)
                                {
                                    LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), " ",
                                        nameof(ClientServiceManager), " ", MethodBase.GetCurrentMethod().Name, " Bulut Log Error Tablosuna insert edilemedi!"));
                                }
                                continue;
                            }
                        }

                        var bankSlip = new BankSlip
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = 2,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var itemCredit = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankCreditAccountInfo.BankRef,
                            BankaccCode = bankCreditAccountInfo.BankAccountCode,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Sign = 1,
                            Credit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            BankProcType = 2,
                            BnCrdtype = bankCreditAccountInfo.BnCardType,
                            CurrselTrans = 2,
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            DueDate = paymentListItem.PaymentDate,
                        };
                        if (paymentListItem.AccountCurrencyCode != "TRY")
                        {
                            itemCredit.CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 :
                                paymentListItem.AccountCurrencyCode == "EUR" ? 20 :
                                paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0;
                        }

                        decimal? itemDebitAmount = 0;

                        if (bankCreditAccountInfo.CurrencyCode != "TL" && bankDebitAccountInfo.CurrencyCode != "TL")
                        {
                            itemDebitAmount = (Math.Abs(paymentListItem.Amount) * curr) / debitCurr;
                        }
                        else if (bankCreditAccountInfo.CurrencyCode == "TL" && bankDebitAccountInfo.CurrencyCode != "TL")
                        {
                            itemDebitAmount = Math.Abs(paymentListItem.Amount) / debitCurr;
                        }
                        else if (bankCreditAccountInfo.CurrencyCode != "TL" && bankDebitAccountInfo.CurrencyCode == "TL")
                        {
                            itemDebitAmount = Math.Abs(paymentListItem.Amount) * debitCurr;
                        }
                        else if (bankCreditAccountInfo.CurrencyCode == "TL" && bankDebitAccountInfo.CurrencyCode == "TL")
                        {
                            itemDebitAmount = Math.Abs(paymentListItem.Amount);
                        }
                        var itemDebit = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankDebitAccountInfo.BankRef,
                            BankaccCode = bankDebitAccountInfo.BankAccountCode,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            //Debit = bankCreditAccountInfo.CurrencyCode == "TL" ? Math.Round(Math.Abs(paymentListItem.Amount) / debitCurr, 2) : Math.Round(Math.Abs(paymentListItem.Amount) * debitCurr, 2),
                            //Amount = bankCreditAccountInfo.CurrencyCode == "TL" ? Math.Round(Math.Abs(paymentListItem.Amount) / debitCurr, 2) : Math.Round(Math.Abs(paymentListItem.Amount) * debitCurr, 2),
                            Debit = itemDebitAmount,
                            Amount = itemDebitAmount,
                            TcAmount = itemDebitAmount,
                            TcXrate = bankDebitAccountInfo.CurrencyCode == "TL" ? 1 : debitCurr,
                            //TcAmount = bankCreditAccountInfo.CurrencyCode == "TL" ? Math.Round(Math.Abs(paymentListItem.Amount) / debitCurr, 2) : bankDebitAccountInfo.CurrencyCode != "TL" ? Math.Round(Math.Abs(paymentListItem.Amount), 2) : Math.Round(Math.Abs(paymentListItem.Amount) * debitCurr, 2),
                            //TcAmount = bankCreditAccountInfo.CurrencyCode == "TL" ? paymentListItem.AccountCurrencyCode != "TRY" ? Math.Abs(paymentListItem.Amount) : Math.Round(Math.Abs(paymentListItem.Amount) / debitCurr) : paymentListItem.AccountCurrencyCode != "TRY" ? Math.Abs(paymentListItem.Amount) : Math.Round(Math.Abs(paymentListItem.Amount) * debitCurr),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : debitCurr,
                            RcAmount = Math.Abs(paymentListItem.Amount) * debitCurr
                            ,//credit == null ? Math.Abs(paymentListItem.Amount) : Math.Abs(credit.Amount),
                            BankProcType = 2,
                            CurrTrans = bankDebitAccountInfo.CurrencyType,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BnCrdtype = bankDebitAccountInfo.BnCardType,
                            CurrselTrans = 2,
                            DueDate = paymentListItem.PaymentDate,
                        };
                        bankSlip.Transactions.items.Add(itemCredit);
                        bankSlip.Transactions.items.Add(itemDebit);
                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);

                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu virman ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }

                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        if (bankDebitAccountInfo.BankAccountCode == null)
                        {
                            var message = string.Concat(paymentListItem.SenderFirmBankIBAN,
                                " nolu Karşı IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), " ", message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject,body: message);
                        }

                        if (bankCreditAccountInfo.BankAccountCode == null)
                        {
                            var message = string.Concat(paymentListItem.FirmBankIBAN,
                                " nolu Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                    }

                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
                    if (!serviceResult.Success)
                    {
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                    }

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }

            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Virman İşlemleri Bitiş------------->"));
        }

        /// <summary>
        /// Çek İşlemleri - Kendi Çekimiz
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        public static void CheckTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Çek İşlemleri Başlangıç------------->"));
            var error = new TransferErrorLog();
            decimal curr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Çek kayıtları sorgulanıyor..."));
                var paymentListItems = GetCheckTransferOnBulut(begDate, endDate,taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet çek kaydı bulundu...") : "Çek kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {

                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Çek Ödemeleri",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                    curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();

                    var bankCreditAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    //var accountRef = DataLogic.GetEmuAccRefBankAccountCode(bankCreditAccountInfo.BankAccountCode);
                    //var bnAccountRef = DataLogic.GetEmuAccRefBankAccountRef(bankCreditAccountInfo.BankRef);
                    //var bankDebitAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.SenderFirmBankIBAN);
                    var doc = paymentListItem.Amount < 0 ? 3 : 1;
                    var checkInfo = DataLogic.GetCheckRefByCheckNo(paymentListItem.CheckNumber, doc);

                    //if (!string.IsNullOrEmpty(bankCreditAccountInfo.BankAccountCode) && !string.IsNullOrEmpty(bankDebitAccountInfo.BankAccountCode) && checkInfo.CheckRef != 0)
                    //if (!string.IsNullOrEmpty(bankCreditAccountInfo.BankAccountCode) && checkInfo.CheckRef != 0)
                    if (checkInfo.CheckRef != 0)
                    {
                        var bankSlip = new CheckSlip()
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = paymentListItem.Amount < 0 ? 11 : 9,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            ProcType = paymentListItem.Amount < 0 ? 2 : 3,//checkInfo.ProcType,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation,
                            DegActive = paymentListItem.AccountCurrencyCode != "TRY" ? 1 : 0,
                            DegCurr = paymentListItem.AccountCurrencyCode != "TRY" ? 1 : 0,

                        };
                        var transactionItem = new TransactionItem
                        {
                            Type = paymentListItem.Amount < 0 ? 3 : 1,
                            CurrentStatus = 8,//checkInfo.CurrStat,
                            //BankCode = bankCreditAccountInfo.BankAccountCode,
                            //AccountRef = accountRef,
                            //BnAccountRef = bnAccountRef,
                            Number = checkInfo.PortfoyNo,
                            Date = paymentListItem.PaymentDate,
                            DueDate = paymentListItem.PaymentDate,
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrselTrans = 2,
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            CsRef = checkInfo.CheckRef,
                            TransStatus = 8,
                            CardMd = 5,
                            CardRef = DataLogic.GetCheckClientRefByCheckRef(checkInfo.CheckRef),
                            SerialNr = paymentListItem.CheckNumber,
                            AffectRisk = 1,
                            CsIban = paymentListItem.FirmBankIBAN

                            //Tranno = "~",
                            //BankRef = bankCreditAccountInfo.BankRef,
                            //BankaccCode = bankCreditAccountInfo.BankAccountCode,

                            //Date = paymentListItem.PaymentDate,
                            //Description = paymentListItem.Explanation,
                            //Sign = 1,
                            //Credit = Math.Abs(paymentListItem.Amount),
                            //Amount = Math.Abs(paymentListItem.Amount),
                            //TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            //TcAmount = Math.Abs(paymentListItem.Amount),
                            //RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            //RcAmount = Math.Abs(paymentListItem.Amount),
                            //BankProcType = 2,
                            //BnCrdtype = 1,
                            //CurrselTrans =
                            //paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,

                        };
                        var bankTransactionItem = new BankTransactionItem
                        {
                            Type = 4,
                            TranNo = "~",
                            //BankAccCode = bankCreditAccountInfo.BankAccountCode,
                            //AccountRef = accountRef,
                            //BnAccountRef = bnAccountRef,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Debit = Math.Abs(paymentListItem.Amount),
                            Credit = Math.Abs(paymentListItem.Amount),
                            Sign = 1,
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrselTrans = 2,
                            GrpFirmTrans = 1,
                            //BnCrdType = bankCreditAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankCreditAccountInfo.BankAccountCode),
                        };

                        bankSlip.Transactions.items.Add(transactionItem);
                        bankSlip.BankTransactions.items.Add(bankTransactionItem);
                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateCheckSlip(bankSlip, TigerDataType.ChequeAndPnoteRolls);
                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu çek ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo 03.05.2021
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                            // todo 28.09.2021 çek virman işlemi kapatıldı ozanayın yönlendirmesi ile
                            //if (bankSlip.ProcType != 2)
                            //{
                            //    ChequeBankTransferSendLogo(paymentListItem);
                            //}
                            //if (bankSlip.ProcType == 2)
                            //{
                            //    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                            //            " nolu kendi çekimiz kaydı için virman oluşturulmaz."));
                            //}
                            // todo 28.09.2021 çek virman işlemi kapatıldı ozanayın yönlendirmesi ile
                        }

                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        if (checkInfo.CheckRef == 0)
                        {
                            var message = string.Concat(paymentListItem.CheckNumber,
                                " nolu çek numarasına ait çek kaydı bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                    }

                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
                    if (!serviceResult.Success)
                    {
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                    }

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }

            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Çek İşlemleri Bitiş------------->"));
        }

        /// <summary>
        /// Banka Hizmet Faturası İşlemleri
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        /// <returns></returns>
        public static void BankServiceInvoiceTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Banka Hizmet Faturası İşlemleri Başlangıç------------->"));
            var error = new TransferErrorLog();
            decimal curr = 0;
            decimal rcRate = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Banka hizmet faturası kayıtları sorgulanıyor..."));
                var paymentListItems = GetBankServiceInvoiceTransferOnBulut(begDate, endDate,taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet masraf kaydı bulundu...", JsonConvert.SerializeObject(paymentListItems)) : "Banka Hizmet Faturası kaydı bulunamadı"));

                foreach (var paymentListItem in paymentListItems)
                {
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Banka Hizmet Faturası",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    if (paymentListItem.AccountCurrencyCode != "TRY")
                    {
                        var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                        curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                        rcRate = DataLogic.GetDailyExchange(1, paymentListItem.PaymentDate).RATES1.ToDecimal();
                        if (curr == 1)
                        {
                            var message = string.Concat(paymentListItem.PaymentID,
                                " işlem kodlu ödeme kaydına istinaden logoda günlük döviz kuru bulunamadı!");
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), string.Concat(nameof(ClientServiceManager), " ", MethodBase.GetCurrentMethod().Name, " ", message)));
                            error.ERROR = message;
                            var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                            if (!insertReslut.Success)
                            {
                                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                    StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                            }
                            continue;
                        }

                    }

                    var bankServiceInvoiceInfo = DataLogic.GetServiceCardPairInfoByPaymetExpCode(paymentListItem.PaymentExpCode);
                    var bankAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);

                    if (bankServiceInvoiceInfo != null && bankAccountInfo.BankAccountCode != null)
                    {
                        var bankServiceInvoiceSlip = new BankServiceSlip
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Type = paymentListItem.Amount < 0 ? 16 : 17,//bankServiceInvoiceInfo.CardType == 1 ? 16 : 17,
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,//bankServiceInvoiceInfo.CardType == 1 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation,
                            Bnacccode = bankAccountInfo.BankAccountCode,

                        };
                        var transactionItem = new BankServiceSlipTransactionItem
                        {
                            Type = 1,
                            Tranno = "~",
                            BankaccCode = bankAccountInfo.BankAccountCode,
                            DocNumber = "A",
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,//bankServiceInvoiceInfo.CardType == 1 ? 1 : 0,
                            Date = paymentListItem.PaymentDate,
                            DueDate = paymentListItem.PaymentDate,
                            //Credit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? rcRate : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BnCrdtype = bankAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankAccountInfo.BankAccountCode),
                            BankProcType = 2
                        };
                        var bankAttachmentInvoiceItem = new AttachmentInvoiceItem
                        {
                            Type = paymentListItem.Amount < 0 ? 4 : 9,
                            Number = "~",
                            Date = paymentListItem.PaymentDate,
                            DocDate = paymentListItem.PaymentDate,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            DocNumber = "A",
                            Notes1 = paymentListItem.Explanation,
                            CurrInvoice = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? rcRate : curr,
                            CurrselTotals = 2,
                            CurrselDetails = 2
                        };
                        var bankAttachmentInvoiceTransactionItem = new AttachmentInvoiceTransactionItem
                        {
                            Type = 4,
                            MasterCode = bankServiceInvoiceInfo.SrvCode,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Quantity = 1,
                            //Price = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? rcRate : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            UnitCode = "ADET",
                            EdtCurr = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            EdtPrice = Math.Abs(paymentListItem.Amount),

                        };
                        bankServiceInvoiceSlip.Transactions.items.Add(transactionItem);
                        bankServiceInvoiceSlip.AttachmentInvoice.items.Add(bankAttachmentInvoiceItem);
                        bankAttachmentInvoiceItem.Transactions.items.Add(bankAttachmentInvoiceTransactionItem);

                        var json = JsonConvert.SerializeObject(bankServiceInvoiceSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankServiceInvoiceSlip(bankServiceInvoiceSlip, TigerDataType.BankSlips);
                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu banka hizmet faturası ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;

                            // todo ilgili tamamlandı olarak geri gönder
                        }

                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        if (bankServiceInvoiceInfo == null)
                        {
                            var message = string.Concat(paymentListItem.SenderFirmBankIBAN,
                                " nolu Karşı IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), " ", message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }

                        if (bankAccountInfo.BankAccountCode == null)
                        {
                            var message = string.Concat(paymentListItem.FirmBankIBAN,
                                " nolu Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }


                    }

                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
                    if (!serviceResult.Success)
                    {
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                    }

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }

            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Banka Hizmet Faturası İşlemleri Bitiş------------->"));
        }

        /// <summary>
        /// Leasing Taksit Ödemesi İşlemleri
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        public static void LeasingPaySendingTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Leasing Taksit İşlemleri Başlangıç------------->"));
            BankPaymentListItem credit = null;
            var error = new TransferErrorLog();

            decimal curr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Leasing taksit kayıtları sorgulanıyor..."));
                var paymentListItems = GetLeasingPaySendingTransferOnBulut(begDate, endDate,taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet taksit kaydı bulundu...") : "Leasing taksit kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Leasing Taksit Ödemesi",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                    curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                    var leasingRef = DataLogic.GetLeasingRefByRegNumber(paymentListItem.CustomField1);
                    //var leasingInfo = DataLogic.GetLeasingPaymetInfo(leasingRef, DateTime.Now);//todo tarihi bulut paymentDate İle değiştir.
                    var leasingInfo = DataLogic.GetLeasingPaymetInfo(leasingRef, paymentListItem.PaymentDate);

                    var bankAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (!string.IsNullOrEmpty(leasingInfo.CODE) && !string.IsNullOrEmpty(bankAccountInfo.BankAccountCode) && leasingRef > 0)
                    {
                        var bankSlip = new BankSlip
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = paymentListItem.Amount < 0 ? 4 : 3,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var item = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankAccountInfo.BankRef,
                            BankaccCode = bankAccountInfo.BankAccountCode,
                            ArpCode = leasingInfo.CODE,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Credit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BankProcType = 2,
                            BnCrdtype = bankAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankAccountInfo.BankAccountCode),
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };
                        bankSlip.Transactions.items.Add(item);
                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);
                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu leasing taksit ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            var message = string.Empty;
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", paymentListItem.CustomField1, " nolu leasing işlemine istinaden ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();

                            leasingInfo.BNFCHREF = result.LogRef;
                            var leasingPaymet = DataLogic.InsertLeasingPayment(leasingInfo);
                            if (leasingPaymet.RowCount > 0)
                            {
                                var updateBnFiche = DataLogic.UpdateBnFicheLeasingPayment(leasingRef, result.LogRef);
                                if (updateBnFiche > 0)
                                {
                                    message = string.Concat(paymentListItem.CustomField1, " nolu taksit hareketi ödemesi", result.ObjectNo,
                                        " fiş numaralı banka fişine bağlandı.");
                                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", message));
                                }
                                else
                                {
                                    message = string.Concat(paymentListItem.CustomField1, " nolu taksit hareketi ödemesi insert edilemedi!");
                                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", message));
                                }
                            }
                            else
                            {
                                message = string.Concat(paymentListItem.CustomField1, " nolu taksit hareketi öedemesi insert edilemedi!",
                                    result.JResult);
                                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", message));
                            }

                            error.ERPRESPONSE += message;
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }

                        LogHelper.Log(result.Success
                            ? error.ERPTRANSFEREXP
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {

                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!", paymentListItem.PaymentID, Environment.NewLine, paymentJson));
                        error.ERROR = string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!");
                        if (leasingInfo.CODE == null)
                        {
                            var message = string.Concat(
                                 paymentListItem.CustomField1,
                                 " nolu leasing kaydı bulunamamıştır! ", paymentListItem.PaymentID,
                                 " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                        if (string.IsNullOrEmpty(bankAccountInfo.Iban))
                        {
                            var message = string.Concat(
                                 paymentListItem.PaymentExpCode, StringUtil.Seperator,
                                 paymentListItem.FirmBankIBAN,
                                 " nolu PN kodlu ve Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!",
                                 paymentListItem.PaymentID,
                                 " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }

                        if (leasingRef == 0)
                        {
                            var message = string.Concat(paymentListItem.CustomField1, " numarasına ait leasing kaydı bulunamadı.");
                            LogHelper.Log(message);
                            error.ERROR = message;
                        }
                    }
                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Leasing Taksit İşlemleri Bitiş------------->"));
        }

        /// <summary>
        /// Kredi Taksit Ödemesi İşlemleri
        /// </summary>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="taxNumber"></param>
        public static void CreditPaySendingTransferSendLogo(DateTime begDate, DateTime endDate, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Kredi Taksit İşlemleri Başlangıç------------->"));
            BankPaymentListItem credit = null;
            var error = new TransferErrorLog();

            decimal curr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Kredi taksit kayıtları sorgulanıyor...", begDate.Date.ToString(), endDate.Date.ToString()));
                var paymentListItems = GetCreaditPaySendingTransferOnBulut(begDate, endDate,taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet taksit kaydı bulundu...") : "Kredi taksit kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Error, nameof(DataLogic), " ", MethodBase.GetCurrentMethod().Name, " ", exception.ErrorDesc, JsonConvert.SerializeObject(paymentListItem, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })));
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Kredi Taksit Ödemesi",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                    curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                    var creditRef = DataLogic.GetBankCreditRefByCreditNumber(paymentListItem.CustomField2);
                    //var leasingInfo = DataLogic.GetLeasingPaymetInfo(leasingRef, DateTime.Now);//todo tarihi bulut paymentDate İle değiştir.
                    var creditInfo = DataLogic.GetBankCreditPaymetInfo(creditRef, paymentListItem.PaymentDate);
                    var creditBankAccountInfo = DataLogic.GetCreditBankAccountInfo(creditInfo.BNCRACCREF);
                    var bankAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (!string.IsNullOrEmpty(creditInfo.CODE) && !string.IsNullOrEmpty(bankAccountInfo.BankAccountCode) && creditRef > 0 && !string.IsNullOrEmpty(creditBankAccountInfo))
                    {
                        var bankSlip = new BankSlip
                        {
                            DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = paymentListItem.Amount < 0 ? 4 : 3,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var item = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankAccountInfo.BankRef,
                            BankaccCode = bankAccountInfo.BankAccountCode,
                            ArpCode = creditInfo.CODE,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Sign = 1,
                            Credit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BankProcType = 2,
                            BnCrdtype = bankAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankAccountInfo.BankAccountCode),
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };
                        var creditItem = new Transaction
                        {
                            Type = 6,
                            Tranno = "~",
                            BankRef = bankAccountInfo.BankRef,
                            BankaccCode = creditBankAccountInfo,
                            ArpCode = creditInfo.CODE,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Debit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            BankProcType = 2,
                            BnCrdtype = bankAccountInfo.BnCardType,//DataLogic.GetBnCardTypeByBankAccountCode(bankAccountInfo.BankAccountCode),
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };

                        bankSlip.Transactions.items.Add(item);
                        bankSlip.Transactions.items.Add(creditItem);
                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);
                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                            UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu kredi taksit ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                        }
                        else
                        {
                            var message = string.Empty;
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", paymentListItem.CustomField1, " nolu leasing işlemine istinaden ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();

                            creditInfo.BNFCHREF = result.LogRef;
                            var leasingPaymet = DataLogic.InsertBankCreditPayment(creditInfo);
                            if (leasingPaymet.RowCount > 0)
                            {
                                var updateBnFiche = DataLogic.UpdateBnFicheLeasingPayment(creditRef, result.LogRef);
                                if (updateBnFiche > 0)
                                {
                                    message = string.Concat(paymentListItem.CustomField1, " nolu taksit hareketi ödemesi", result.ObjectNo,
                                        " fiş numaralı banka fişine bağlandı.");
                                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", message));
                                }
                                else
                                {
                                    message = string.Concat(paymentListItem.CustomField1, " nolu taksit hareketi ödemesi insert edilemedi!");
                                    LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", message));
                                }
                            }
                            else
                            {
                                message = string.Concat(paymentListItem.CustomField1, " nolu taksit hareketi öedemesi insert edilemedi!",
                                    result.JResult);
                                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToString(), " ", message));
                            }

                            error.ERPRESPONSE += message;
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }

                        LogHelper.Log(result.Success
                            ? error.ERPTRANSFEREXP
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!", paymentListItem.PaymentID, Environment.NewLine, paymentJson));
                        error.ERROR = string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.PaymentExpCode, " kodlu cari hesap bulunamadı!");
                        if (creditInfo.CODE == null)
                        {
                            var message = string.Concat(
                                 paymentListItem.CustomField1,
                                 " nolu leasing kaydı bulunamamıştır! ", paymentListItem.PaymentID,
                                 " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                        if (string.IsNullOrEmpty(creditBankAccountInfo))
                        {
                            var message = string.Concat(creditInfo.BNCRACCREF, " - ", creditInfo.CODE,
                                 " nolu krediye ait banka bilgisi bulunamamıştır! ", paymentListItem.PaymentID,
                                 " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }
                        if (string.IsNullOrEmpty(bankAccountInfo.Iban))
                        {
                            var message = string.Concat(
                                 paymentListItem.PaymentExpCode, StringUtil.Seperator,
                                 paymentListItem.FirmBankIBAN,
                                 " nolu PN kodlu ve Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!",
                                 paymentListItem.PaymentID,
                                 " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                            error.ERROR = message;
                            //UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        }

                        if (creditRef == 0)
                        {
                            var message = string.Concat(paymentListItem.CustomField1, " numarasına ait kredi kaydı bulunamadı.");
                            LogHelper.Log(message);
                            error.ERROR = message;
                        }
                    }
                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);

                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Kredi Taksit İşlemleri Bitiş------------->"));
        }

        /// <summary>
        /// Çek Ödemesi Virman İşlemi
        /// </summary>
        /// <param name="paymentListItem"></param>
        /// <param name="taxNumber"></param>
        public static void ChequeBankTransferSendLogo(BankPaymentListItem paymentListItem, string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Çek Ödeme Virmanı İşlemleri Başlangıç------------->"));
            BankPaymentListItem credit = null;
            var error = new TransferErrorLog();
            decimal curr = 0;
            var json = string.Empty;
            var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);

            try
            {
                error = new TransferErrorLog
                {
                    PAYMENTID = paymentListItem.PaymentID,
                    FICHEDATE = paymentListItem.PaymentDate,
                    PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                    BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                    LOGOFICHETYPE = "Çek Virman Fişi",
                    SENDERFIRMNAME = paymentListItem.SenderFirmName,
                    SENDERBANK = paymentListItem.SenderFirmBankName,
                    SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                    BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                    FIRMBANK = paymentListItem.FirmBankName,
                    FIRMIBAN = paymentListItem.FirmBankIBAN,
                    EXPLANATION = paymentListItem.Explanation
                };
                if (paymentListItem.AccountCurrencyCode != "TRY")
                {
                    curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                    if (curr == 1)
                    {
                        var message = string.Concat(paymentListItem.PaymentID,
                            " işlem kodlu ödeme kaydına istinaden logoda günlük döviz kuru bulunamadı!");
                        UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: message);
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), string.Concat(nameof(ClientServiceManager), " ", MethodBase.GetCurrentMethod().Name, " ", message)));
                        error.ERROR = message;
                        var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                        if (!insertReslut.Success)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                        }
                    }
                }

                var crossBankTransferCreditAccountInfo = DataLogic.GetCrossBankTransferAccountCode(paymentListItem.FirmBankIBAN, currencyType.Value);
                var bankCreditAccountInfo = new BankAccountDto();
                if (!string.IsNullOrEmpty(crossBankTransferCreditAccountInfo.BankAccountCode))
                {
                    bankCreditAccountInfo = DataLogic.GetBankAccountInfoByBankAccountCode(string.Concat("10", crossBankTransferCreditAccountInfo.BankAccountCode.Substring(2)));
                }

                if (!string.IsNullOrEmpty(crossBankTransferCreditAccountInfo.BankAccountCode))
                {
                    var bankSlip = new BankSlip
                    {
                        DataObjectParameter = new DataObjectParameter(),
                        Date = paymentListItem.PaymentDate,
                        Number = "~",
                        Type = 2,
                        AuxilCode = paymentListItem.PaymentID.ToString(),
                        Sign = paymentListItem.Amount < 0 ? 1 : 0,
                        CurrselTotals = 1,
                        CurrselDetails = 2,
                        Notes1 = paymentListItem.Explanation
                    };
                    var itemCredit = new Transaction
                    {
                        Type = 1,
                        Tranno = "~",
                        //BankRef = bankCreditAccountInfo.BankRef,
                        BankaccCode = crossBankTransferCreditAccountInfo.BankAccountCode,
                        Date = paymentListItem.PaymentDate,
                        Description = paymentListItem.Explanation,
                        Sign = 1,
                        Credit = Math.Abs(paymentListItem.Amount),
                        Amount = Math.Abs(paymentListItem.Amount),
                        TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                        TcAmount = Math.Abs(paymentListItem.Amount),
                        RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                        RcAmount = Math.Abs(paymentListItem.Amount),
                        BankProcType = 2,
                        BnCrdtype = crossBankTransferCreditAccountInfo.BnCardType,
                        CurrselTrans = 2,
                        CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                        DueDate = paymentListItem.PaymentDate,
                    };
                    if (paymentListItem.AccountCurrencyCode != "TRY")
                    {
                        itemCredit.CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 :
                            paymentListItem.AccountCurrencyCode == "EUR" ? 20 :
                            paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0;
                    }
                    var itemDebit = new Transaction
                    {
                        Type = 1,
                        Tranno = "~",
                        BankRef = bankCreditAccountInfo.BankRef,
                        //BankaccCode = string.Concat("10", crossBankTransferCreditAccountInfo.Substring(2, crossBankTransferCreditAccountInfo.Length - 2)),
                        BankaccCode = string.Concat("10", crossBankTransferCreditAccountInfo.BankAccountCode.Substring(2)),
                        Date = paymentListItem.PaymentDate,
                        Description = paymentListItem.Explanation,
                        Debit = Math.Abs(paymentListItem.Amount),//credit == null ? Math.Abs(paymentListItem.Amount) : Math.Abs(credit.Amount),
                        Amount = Math.Abs(paymentListItem.Amount),//credit == null ? Math.Abs(paymentListItem.Amount) : Math.Abs(credit.Amount),
                        TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                        TcAmount = Math.Abs(paymentListItem.Amount),//credit == null ? Math.Abs(paymentListItem.Amount) : Math.Abs(credit.Amount),
                        RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                        RcAmount = Math.Abs(paymentListItem.Amount),//credit == null ? Math.Abs(paymentListItem.Amount) : Math.Abs(credit.Amount),
                        BankProcType = 2,
                        CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                        BnCrdtype = bankCreditAccountInfo.BnCardType,
                        CurrselTrans = 2,
                        DueDate = paymentListItem.PaymentDate,
                    };
                    bankSlip.Transactions.items.Add(itemCredit);
                    bankSlip.Transactions.items.Add(itemDebit);
                    json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    error.ERPPOSTJSON = json;

                    var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);

                    if (!result.Success)
                    {
                        error.ERPRESPONSE = result.JResult.ToString();
                        UtilitiesMail.SendMail(GlobalSettings.MailTo, subject: GlobalSettings.MailSubject, body: string.Concat(paymentListItem.PaymentID, " işlem kodlu çek virmanı ödeme hareketi logoya aktarılamamıştır. ", result.JResult.ToString()));
                    }
                    else
                    {
                        error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                            " nolu banka fişi logoya aktarılmıştır.");
                        error.ERPFICHENO = result.ObjectNo.ToString();
                        error.ERPRESPONSE = result.ToJsonString();
                    }

                    LogHelper.Log(result.Success
                        ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                            " nolu banka fişi logoya aktarılmıştır.")
                        : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                            result.JResult));
                }
                else
                {
                    var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    error.PAYMENTDATA = paymentJson;
                    error.ERPPOSTJSON = json;
                    if (string.IsNullOrEmpty(bankCreditAccountInfo.BankAccountCode))
                    {
                        var message = string.Concat(LogHelper.LogType.Error.ToLogType(),
                            paymentListItem.FirmBankIBAN,
                            " nolu Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                            " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), message));
                        error.ERROR = message;
                    }

                    if (string.IsNullOrEmpty(crossBankTransferCreditAccountInfo.BankAccountCode))
                    {
                        var message = string.Concat(LogHelper.LogType.Error.ToLogType(),
                            paymentListItem.FirmBankIBAN,
                            " nolu banka hesap bilgisi bulunamamıştır!",
                            paymentListItem.PaymentID,
                            " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson);
                        LogHelper.Log(message);
                        error.ERROR = message;
                    }
                }

                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
                if (!serviceResult.Success)
                {
                    LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                        nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                        StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }

            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Çek Ödeme Virmanı İşlemleri Bitiş------------->"));
        }

        /// <summary>
        /// Farklı döviz türleri arasında alım satım işlemi (Arbitraj)
        /// </summary>
        /// <param name="paymentListItems"></param>
        public static void ArbitrajBankTransferSendLogo(DateTime begDate, DateTime endDate,string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Arbitraj İşlemleri Başlangıç------------->"));
            BankPaymentListItem credit = null;
            var error = new TransferErrorLog();
            decimal curr = 0;
            decimal debitCurr = 0;
            decimal parite = 0;
            double reportCurr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Arbitraj kayıtları sorgulanıyor..."));
                var paymentListItems = GetArbitajTransferOnBulut(begDate, endDate,taxNumber);

                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet Arbitraj kaydı bulundu...") : "Arbitraj kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {
                    var arbitrajCredit = GetArbitrajCreditBankTransferOnBulut(paymentListItem.PaymentDate,
                        paymentListItem.CustomField2, paymentListItem.FirmBankName);
                    if (arbitrajCredit == null)
                    {
                        var message = string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, paymentListItem.PaymentID, StringUtil.Seperator,
                            " işlem numaralı kayda ait karşı hareket bulunamamıştır.");
                        LogHelper.Log(message);
                        error.ERROR = message;
                        var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                        if (!insertReslut.Success)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                        }

                        continue;
                    }
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Arbitraj Muhasebe Fişi",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    var bankCreditAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(arbitrajCredit.FirmBankIBAN);
                    var bankDebitAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (paymentListItem.AccountCurrencyCode != "TRY")
                    {
                        reportCurr = DataLogic.GetDailyExchange(FirmReportCurrencyType, paymentListItem.PaymentDate).RATES1;
                        var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                        curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();//GetExchangeDebitBankTransferOnBulut(paymentListItem.PaymentDate, paymentListItem.VoucherNumber, paymentListItem.FirmBankName);
                        var creditCurrencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == arbitrajCredit.AccountCurrencyCode);
                        debitCurr = DataLogic.GetDailyExchange(creditCurrencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();
                        if (curr != 1 && curr > 1)
                        {
                            parite = Math.Abs(paymentListItem.Amount / arbitrajCredit.Amount);
                        }
                        if (curr == 1)
                        {
                            var message = string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, paymentListItem.PaymentID, StringUtil.Seperator,
                                " işlem numaralı arbitraj kaydına istinaden logoda günlük döviz kurları bulunamadı.");
                            LogHelper.Log(message);
                            error.ERROR = message;
                            var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                            if (!insertReslut.Success)
                            {
                                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                    StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                            }
                            continue;
                        }
                    }
                    if (bankCreditAccountInfo.BankAccountCode != null && bankDebitAccountInfo.BankAccountCode != null)
                    {
                        var glSlip = new GlSlip
                        {
                            //DataObjectParameter = new DataObjectParameter(),
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = 4,
                            AuxilCode = "FORWARD AR",//paymentListItem.PaymentID.ToString(),
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var itemCredit = new GlSlipTransactionItem
                        {
                            Date = arbitrajCredit.PaymentDate,
                            Sign = 1,
                            GlCode = DataLogic.GetEmuAccRefBankAccount(bankCreditAccountInfo.Id, 6, 1),
                            Credit = Math.Round(Math.Abs(arbitrajCredit.Amount * debitCurr).ToDouble(), 4),
                            Description = arbitrajCredit.Explanation,
                            CurrTrans = bankCreditAccountInfo.CurrencyType,
                            RcXrate = reportCurr, //arbitrajCredit.AccountCurrencyCode == "TRY" ? 1 : curr.ToDouble(),
                            RcAmount = Math.Abs(arbitrajCredit.Amount).ToDouble() / reportCurr,
                            TcXrate = arbitrajCredit.AccountCurrencyCode == "TRY" ? 1 : debitCurr.ToDouble(),
                            TcAmount = Math.Abs(arbitrajCredit.Amount).ToDouble(),
                            CurrselTrans = 2,
                            DocDate = arbitrajCredit.PaymentDate,
                            LineType = 2,
                            CodeRef = bankCreditAccountInfo.Id
                        };
                        var itemDebit = new GlSlipTransactionItem
                        {
                            Date = paymentListItem.PaymentDate,
                            GlCode = DataLogic.GetEmuAccRefBankAccount(bankDebitAccountInfo.Id, 6, 1),
                            Debit = Math.Round((Math.Abs(paymentListItem.Amount) * curr).ToDouble(), 4),
                            Description = paymentListItem.Explanation,
                            CurrTrans = bankDebitAccountInfo.CurrencyType, //paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            RcXrate = reportCurr,//paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr.ToDouble(),
                            RcAmount = Math.Abs(paymentListItem.Amount).ToDouble(),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr.ToDouble(),
                            TcAmount = Math.Abs(paymentListItem.Amount).ToDouble(),
                            CurrselTrans = 2,
                            DocDate = paymentListItem.PaymentDate,
                            LineType = 2,
                            CodeRef = bankDebitAccountInfo.Id

                        };


                        double accountingCreditOrDebit = itemCredit.Credit.ToDouble() - itemDebit.Debit.ToDouble();
                        var itemAccount = new GlSlipTransactionItem
                        {
                            Date = paymentListItem.PaymentDate,
                            Sign = accountingCreditOrDebit < 0 ? 1 : (int?)null,
                            GlCode = accountingCreditOrDebit < 0 ? "646.01.10.0005" : "656.01.10.0005",
                            Credit = Math.Round(accountingCreditOrDebit, 4) < 0 ? Math.Abs(accountingCreditOrDebit) : (double?)null,
                            Debit = Math.Round(accountingCreditOrDebit, 4) > 0 ? Math.Abs(accountingCreditOrDebit) : (double?)null,
                            Description = paymentListItem.Explanation,
                            //CurrTrans = paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : paymentListItem.AccountCurrencyCode == "GBP" ? 17 : 0,
                            RcXrate = reportCurr, //paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr.ToDouble(),
                            RcAmount = Math.Abs(accountingCreditOrDebit / (double)curr).ToDouble(),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr.ToDouble(),
                            TcAmount = Math.Abs(accountingCreditOrDebit),
                            CurrselTrans = 2,
                            DocDate = paymentListItem.PaymentDate,
                        };
                        glSlip.Transactions.items.Add(itemCredit);
                        glSlip.Transactions.items.Add(itemDebit);
                        glSlip.Transactions.items.Add(itemAccount);
                        var json = JsonConvert.SerializeObject(glSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateGlSlip(glSlip, TigerDataType.GLSlips);

                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu muhasebe fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            var debitResponse = UpdatePaymentStatusInfo(arbitrajCredit.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }

                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu muhasebe fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Muhasebe fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.PAYMENTDATA = paymentJson;
                        if (bankDebitAccountInfo == null)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(),
                                paymentListItem.SenderFirmBankIBAN,
                                " nolu Karşı IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson));
                            error.ERROR = string.Concat(paymentListItem.SenderFirmBankIBAN,
                                " nolu Karşı IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!");
                        }

                        if (bankCreditAccountInfo == null)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(),
                                paymentListItem.FirmBankIBAN,
                                " nolu Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson));
                            error.ERROR = string.Concat(paymentListItem.SenderFirmBankIBAN,
                                " nolu Firma IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID,
                                " nolu hareket logo'ya aktarılamamıştır!");
                        }
                    }

                    var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
                    if (!serviceResult.Success)
                    {
                        LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                    StringUtil.Seperator, e.Message));
                error.ERROR = e.Message;
                var serviceResult = DataLogic.InsertTransferErrorLog(error, ref exception);
            }

            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Virman İşlemleri Bitiş------------->"));
        }
        /// <summary>
        /// Döviz Alım İşlemi
        /// </summary>
        /// <param name="paymentListItems"></param>
        public static void BuyExchangeBankTransferSendLogo(DateTime begDate, DateTime endDate,string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Arbitraj İşlemleri Başlangıç------------->"));
            var error = new TransferErrorLog();
            decimal curr = 0;
            decimal debitCurr = 0;
            decimal parite = 0;
            decimal reportCurr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Döviz alım kayıtları sorgulanıyor..."));
                var paymentListItems = GetBuyExchangeBankTransferOnBulut(begDate, endDate,taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet Döviz alım kaydı bulundu...") : "Döviz alım kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {

                    var exchangeCredit = GetBuyExchangeCreditBankTransferOnBulut(paymentListItem.PaymentDate, paymentListItem.CustomField2, paymentListItem.FirmBankName);
                    if (exchangeCredit == null)
                    {

                    }


                    if (exchangeCredit == null)
                    {
                        var message = string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, paymentListItem.PaymentID, StringUtil.Seperator,
                            " işlem numaralı kayda ait karşı hareket bulunamamıştır.");
                        LogHelper.Log(message);
                        error.ERROR = message;
                        var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                        if (!insertReslut.Success)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                        }

                        continue;
                    }
                    else
                    {
                        curr = exchangeCredit.AccountCurrencyCode == "TRY" ? Math.Abs(exchangeCredit.Amount / paymentListItem.Amount) : Math.Abs(paymentListItem.Amount / exchangeCredit.Amount);
                    }
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Döviz Alım Virman Fişi",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    if (paymentListItem.AccountCurrencyCode != "TRY")
                    {
                        reportCurr = DataLogic.GetDailyExchange(FirmReportCurrencyType, paymentListItem.PaymentDate).RATES1.ToDecimal();
                        var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                        //curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();//GetExchangeDebitBankTransferOnBulut(paymentListItem.PaymentDate, paymentListItem.VoucherNumber, paymentListItem.FirmBankName);

                        if (curr == 1)
                        {
                            var message = string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, paymentListItem.PaymentID, StringUtil.Seperator,
                                " işlem numaralı arbitraj kaydına istinaden logoda günlük döviz kurları bulunamadı.");
                            LogHelper.Log(message);
                            error.ERROR = message;
                            var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                            if (!insertReslut.Success)
                            {
                                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                    StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                            }
                            continue;
                        }
                    }


                    var bankCreditAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(exchangeCredit.FirmBankIBAN);
                    var bankDebitAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (bankCreditAccountInfo != null && bankDebitAccountInfo != null)
                    {
                        var bankSlip = new BankSlip
                        {
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = 2,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var itemCredit = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankCreditAccountInfo.BankRef,
                            BankaccCode = bankCreditAccountInfo.BankAccountCode,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Sign = 1,
                            Credit = Math.Abs(exchangeCredit.Amount),
                            Amount = Math.Abs(exchangeCredit.Amount),
                            TcXrate = exchangeCredit.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(exchangeCredit.Amount),
                            RcXrate = reportCurr == 0 ? 1 : reportCurr,
                            RcAmount = Math.Abs(exchangeCredit.Amount),
                            BankProcType = 1,
                            BnCrdtype = bankCreditAccountInfo.BnCardType,
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };
                        var itemDebit = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankDebitAccountInfo.BankRef,
                            BankaccCode = bankDebitAccountInfo.BankAccountCode,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Debit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : reportCurr == 0 ? 1 : reportCurr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = bankDebitAccountInfo.CurrencyType,
                            BankProcType = 1,
                            BnCrdtype = bankDebitAccountInfo.BnCardType,
                            CurrselTrans = 2,
                            DueDate = paymentListItem.PaymentDate
                        };
                        bankSlip.Transactions.items.Add(itemDebit);
                        bankSlip.Transactions.items.Add(itemCredit);

                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);

                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu virman fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            var debitResponse = UpdatePaymentStatusInfo(exchangeCredit.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }
                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu banka fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Banka fişi logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        if (bankDebitAccountInfo == null)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.SenderFirmBankIBAN, " nolu IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID, " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson));
                        }
                        else if (bankCreditAccountInfo == null)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.FirmBankIBAN, " nolu IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID, " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson));
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Döviz Alım İşlemi
        /// </summary>
        /// <param name="paymentListItems"></param>
        public static void SalesExchangeBankTransferSendLogo(DateTime begDate, DateTime endDate,string taxNumber)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "<-------------Arbitraj İşlemleri Başlangıç------------->"));
            var error = new TransferErrorLog();
            decimal curr = 0;
            decimal debitCurr = 0;
            decimal parite = 0;
            decimal reportCurr = 0;
            try
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, "Döviz satış kayıtları sorgulanıyor..."));
                var paymentListItems = GetSalesExchangeBankTransferOnBulut(begDate, endDate,taxNumber);
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), StringUtil.Seperator, paymentListItems.Count != 0 ? string.Concat(paymentListItems.Count.ToString(), " adet Döviz satış kaydı bulundu...") : "Döviz satış kaydı bulunamadı"));
                foreach (var paymentListItem in paymentListItems)
                {

                    var exchangeCredit = GetSalesExchangeCreditBankTransferOnBulut(paymentListItem.PaymentDate, paymentListItem.CustomField2, paymentListItem.FirmBankName);
                    if (exchangeCredit == null)
                    {
                        var message = string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                            nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                            StringUtil.Seperator, paymentListItem.PaymentID, StringUtil.Seperator,
                            " işlem numaralı kayda ait karşı hareket bulunamamıştır.");
                        LogHelper.Log(message);
                        error.ERROR = message;
                        var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                        if (!insertReslut.Success)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                        }

                        continue;
                    }
                    else
                    {
                        curr = exchangeCredit.AccountCurrencyCode == "TRY" ? Math.Abs(exchangeCredit.Amount / paymentListItem.Amount) : Math.Abs(paymentListItem.Amount / exchangeCredit.Amount);
                    }
                    error = new TransferErrorLog
                    {
                        PAYMENTID = paymentListItem.PaymentID,
                        FICHEDATE = paymentListItem.PaymentDate,
                        PAYMENTEXPCODE = paymentListItem.PaymentExpCode,
                        BULUTFICHETYPE = paymentListItem.PaymentTypeExplantion,
                        LOGOFICHETYPE = "Döviz Satış Virman Fişi",
                        SENDERFIRMNAME = paymentListItem.SenderFirmName,
                        SENDERBANK = paymentListItem.SenderFirmBankName,
                        SENDERIBAN = paymentListItem.SenderFirmBankIBAN,
                        BRANCHFIRMNAME = paymentListItem.BranchFirmName,
                        FIRMBANK = paymentListItem.FirmBankName,
                        FIRMIBAN = paymentListItem.FirmBankIBAN,
                        EXPLANATION = paymentListItem.Explanation
                    };
                    if (paymentListItem.AccountCurrencyCode != "TRY")
                    {
                        reportCurr = DataLogic.GetDailyExchange(FirmReportCurrencyType, paymentListItem.PaymentDate).RATES1.ToDecimal();
                        var currencyType = CurrenyTypes.GetCurrenyTypes().FirstOrDefault(x => x.Key == paymentListItem.AccountCurrencyCode);
                        //curr = DataLogic.GetDailyExchange(currencyType.Value, paymentListItem.PaymentDate).RATES1.ToDecimal();//GetExchangeDebitBankTransferOnBulut(paymentListItem.PaymentDate, paymentListItem.VoucherNumber, paymentListItem.FirmBankName);

                        if (curr == 1)
                        {
                            var message = string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                StringUtil.Seperator, paymentListItem.PaymentID, StringUtil.Seperator,
                                " işlem numaralı arbitraj kaydına istinaden logoda günlük döviz kurları bulunamadı.");
                            LogHelper.Log(message);
                            error.ERROR = message;
                            var insertReslut = DataLogic.InsertTransferErrorLog(error, ref exception);
                            if (!insertReslut.Success)
                            {
                                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator,
                                    nameof(ClientServiceManager), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name,
                                    StringUtil.Seperator, "Bulut Log Error Tablosuna insert edilemedi!"));
                            }
                            continue;
                        }
                    }


                    var bankCreditAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(exchangeCredit.FirmBankIBAN);
                    var bankDebitAccountInfo = DataLogic.GetBankAccountInfoByIbanNo(paymentListItem.FirmBankIBAN);
                    if (bankCreditAccountInfo != null && bankDebitAccountInfo != null)
                    {
                        var bankSlip = new BankSlip
                        {
                            Date = paymentListItem.PaymentDate,
                            Number = "~",
                            Type = 2,
                            AuxilCode = paymentListItem.PaymentID.ToString(),
                            Sign = paymentListItem.Amount < 0 ? 1 : 0,
                            CurrselTotals = 1,
                            CurrselDetails = 2,
                            Notes1 = paymentListItem.Explanation
                        };
                        var itemCredit = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankCreditAccountInfo.BankRef,
                            BankaccCode = bankCreditAccountInfo.BankAccountCode,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Sign = 1,
                            Credit = Math.Abs(exchangeCredit.Amount * curr),
                            Amount = Math.Abs(exchangeCredit.Amount * curr),
                            TcXrate = exchangeCredit.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(exchangeCredit.Amount),
                            RcXrate = reportCurr == 0 ? 1 : reportCurr,
                            RcAmount = Math.Abs(exchangeCredit.Amount),
                            CurrTrans = bankCreditAccountInfo.CurrencyType,
                            BankProcType = 1,
                            BnCrdtype = bankCreditAccountInfo.BnCardType,
                            CurrselTrans = 2,//paymentListItem.AccountCurrencyCode == "USD" ? 1 : paymentListItem.AccountCurrencyCode == "EUR" ? 20 : 0,
                            DueDate = paymentListItem.PaymentDate
                        };
                        var itemDebit = new Transaction
                        {
                            Type = 1,
                            Tranno = "~",
                            BankRef = bankDebitAccountInfo.BankRef,
                            BankaccCode = bankDebitAccountInfo.BankAccountCode,
                            Date = paymentListItem.PaymentDate,
                            Description = paymentListItem.Explanation,
                            Debit = Math.Abs(paymentListItem.Amount),
                            Amount = Math.Abs(paymentListItem.Amount),
                            TcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : curr,
                            TcAmount = Math.Abs(paymentListItem.Amount),
                            RcXrate = paymentListItem.AccountCurrencyCode == "TRY" ? 1 : reportCurr == 0 ? 1 : reportCurr,
                            RcAmount = Math.Abs(paymentListItem.Amount),
                            CurrTrans = bankDebitAccountInfo.CurrencyType,
                            BankProcType = 1,
                            BnCrdtype = bankDebitAccountInfo.BnCardType,
                            CurrselTrans = 2,
                            DueDate = paymentListItem.PaymentDate
                        };
                        bankSlip.Transactions.items.Add(itemDebit);
                        bankSlip.Transactions.items.Add(itemCredit);

                        var json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        error.ERPPOSTJSON = json;
                        var result = restServiceManager.CreateBankSlip(bankSlip, TigerDataType.BankSlips);

                        if (!result.Success)
                        {
                            error.ERPRESPONSE = result.JResult.ToString();
                        }
                        else
                        {
                            error.ERPTRANSFEREXP = string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu döviz satış virman fişi logoya aktarılmıştır.");
                            error.ERPFICHENO = result.ObjectNo.ToString();
                            error.ERPRESPONSE = result.ToJsonString();
                            // todo
                            var response = UpdatePaymentStatusInfo(paymentListItem.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            var debitResponse = UpdatePaymentStatusInfo(exchangeCredit.PaymentID, paymentListItem);
                            error.STATUSINFO = response;
                            // todo ilgili tamamlandı olarak geri gönder
                        }
                        LogHelper.Log(result.Success
                            ? string.Concat(LogHelper.LogType.Info.ToString(), " ", result.ObjectNo,
                                " nolu  döviz satış virman fişi logoya aktarılmıştır.")
                            : string.Concat(LogHelper.LogType.Error.ToString(), " ", "Döviz satış virman logoya aktarılamadı!",
                                result.JResult));
                    }
                    else
                    {
                        var paymentJson = JsonConvert.SerializeObject(paymentListItem, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        if (bankDebitAccountInfo == null)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.SenderFirmBankIBAN, " nolu IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID, " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson));
                        }
                        else if (bankCreditAccountInfo == null)
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), paymentListItem.FirmBankIBAN, " nolu IBAN'a ait banka hesap bilgisi bulunamamıştır!", paymentListItem.PaymentID, " nolu hareket logo'ya aktarılamamıştır!", Environment.NewLine, paymentJson));
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        #endregion


    }
}