using System;
using System.Reflection;
using BulutTahsilatIntegration.WinService.BTIIntegration;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.DataAccess;
using BulutTahsilatintegration.WinService.Model.ErpModel;
using BulutTahsilatIntegration.WinService.Model.ErpModel;
using BulutTahsilatIntegration.WinService.Model.Global;
using BulutTahsilatIntegration.WinService.Model.Logging;
using BulutTahsilatIntegration.WinService.Model.ResultTypes;
using Newtonsoft.Json;
using TigerRestService = BulutTahsilatIntegration.WinService.BTIIntegration.TigerRestService;
using TokenHolder = BulutTahsilatIntegration.WinService.BTIIntegration.TokenHolder;

namespace BulutTahsilatIntegration.WinService.ServiceManager
{
    public class RestServiceManager
    {
        private DataAccessException exception;
        private int? postId = 0;
        private int tokenRepeatCount = 0;
        public ServiceResult CreateBankSlip(BankSlip bankSlip, TigerDataType dataType, string firmNr = null)
        {
            //var request = HttpContext.Current.Request;
            var tokenHolder = TokenHolder.GetInstance();
            var restService = new TigerRestService();
            string json = string.Empty;
            var serviceResult = new ServiceResult();
            try
            {
                //orderFiche.DataObjectParameter = new DataObjectParameter();
                json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                postId = DataLogic.InsertPostLog(new PostLog
                {
                    PostDate = DateTime.Now,
                    HostIp = dataType.Value,
                    IdentityName = "",
                    OperationType = "POST",
                    Url = dataType.Value,
                    RequestMethod = "POST",
                    JsonData = json
                }, ref exception).LogRef;
                if (restService.GetAccessToken(firmNr).IsLoggedIn)
                {
                    serviceResult.JResult = restService.HttpPost(dataType.Value, json);
                    if (serviceResult.JResult.HasValues)
                    {
                        if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                        {
                            serviceResult.Success = true;
                            serviceResult.LogRef = serviceResult.JResult["INTERNAL_REFERENCE"].ToObject<int>();
                            serviceResult.ObjectNo = DataLogic.GetBnFicheNumberByRef(serviceResult.LogRef, ref exception);
                            if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                            {

                            }
                        }
                    }

                    DataLogic.InsertResponseLog(new ResponseLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = dataType.Value,
                        IdentityName = "",
                        OperationType = "REQUEST",
                        ResponseData = serviceResult.JResult?.ToString(),
                        JsonData = json
                    }, ref exception);
                }
                else
                {
                    DataLogic.InsertErrorLog(new ErrorLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = dataType.Value,
                        IdentityName = "",
                        OperationType = "POST",
                        ErrorClassName = nameof(ServiceManager),
                        ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                        ErrorMessage = tokenHolder?.Token,
                        ResponseData = tokenHolder?.Token,
                        JsonData = json
                    }, ref exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return serviceResult;
        }

        public ServiceResult CreateCheckSlip(CheckSlip bankSlip, TigerDataType dataType)
        {
            //var request = HttpContext.Current.Request;
            var tokenHolder = TokenHolder.GetInstance();
            var restService = new TigerRestService();
            string json = string.Empty;
            var serviceResult = new ServiceResult();
            try
            {
                //orderFiche.DataObjectParameter = new DataObjectParameter();
                json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                postId = DataLogic.InsertPostLog(new PostLog
                {
                    PostDate = DateTime.Now,
                    HostIp = dataType.Value,
                    IdentityName = "",
                    OperationType = "POST",
                    Url = dataType.Value,
                    RequestMethod = "POST",
                    JsonData = json
                }, ref exception).LogRef;
                if (restService.GetAccessToken().IsLoggedIn)
                {
                    serviceResult.JResult = restService.HttpPost(dataType.Value, json);
                    if (serviceResult.JResult.HasValues)
                    {
                        if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                        {
                            serviceResult.Success = true;
                            serviceResult.LogRef = serviceResult.JResult["INTERNAL_REFERENCE"].ToObject<int>();
                            serviceResult.ObjectNo = DataLogic.GetCheckFicheNumberByRef(serviceResult.LogRef, ref exception);
                            if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                            {

                            }
                        }
                    }

                    DataLogic.InsertResponseLog(new ResponseLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = TigerDataType.ChequeAndPnoteRolls.Value,
                        IdentityName = "",
                        OperationType = "REQUEST",
                        ResponseData = serviceResult.JResult?.ToString(),
                        JsonData = json
                    }, ref exception);
                }
                else
                {
                    DataLogic.InsertErrorLog(new ErrorLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = dataType.Value,
                        IdentityName = "",
                        OperationType = "POST",
                        ErrorClassName = nameof(ServiceManager),
                        ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                        ErrorMessage = serviceResult.JResult.ToString(),
                        ResponseData = serviceResult.JResult.ToString(),
                        JsonData = json
                    }, ref exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return serviceResult;
        }

        public ServiceResult CreateBankServiceInvoiceSlip(BankServiceSlip bankSlip, TigerDataType dataType)
        {
            //var request = HttpContext.Current.Request;
            var tokenHolder = TokenHolder.GetInstance();
            var restService = new TigerRestService();
            string json = string.Empty;
            var serviceResult = new ServiceResult();
            try
            {
                //orderFiche.DataObjectParameter = new DataObjectParameter();
                json = JsonConvert.SerializeObject(bankSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                postId = DataLogic.InsertPostLog(new PostLog
                {
                    PostDate = DateTime.Now,
                    HostIp = dataType.Value,
                    IdentityName = "",
                    OperationType = "POST",
                    Url = dataType.Value,
                    RequestMethod = "POST",
                    JsonData = json
                }, ref exception).LogRef;
                if (restService.GetAccessToken().IsLoggedIn)
                {
                    serviceResult.JResult = restService.HttpPost(dataType.Value, json);
                    if (serviceResult.JResult.HasValues)
                    {
                        if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                        {
                            serviceResult.Success = true;
                            serviceResult.LogRef = serviceResult.JResult["INTERNAL_REFERENCE"].ToObject<int>();
                            serviceResult.ObjectNo = DataLogic.GetBnFicheNumberByRef(serviceResult.LogRef, ref exception);
                            if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                            {

                            }
                        }
                    }

                    DataLogic.InsertResponseLog(new ResponseLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = TigerDataType.BankSlips.Value,
                        IdentityName = "",
                        OperationType = "REQUEST",
                        ResponseData = serviceResult.JResult?.ToString(),
                        JsonData = json
                    }, ref exception);

                }
                else
                {
                    DataLogic.InsertErrorLog(new ErrorLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = dataType.Value,
                        IdentityName = "",
                        OperationType = "POST",
                        ErrorClassName = nameof(ServiceManager),
                        ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                        ErrorMessage = serviceResult.JResult.ToString(),
                        ResponseData = serviceResult.JResult.ToString(),
                        JsonData = json
                    }, ref exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return serviceResult;
        }

        public ServiceResult CreateData(int postId, string dataType, string jsonData)
        {
            LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), "<----------------------Start Create Data------------------->"));
            var tokenHolder = TokenHolder.GetInstance();
            var restService = new TigerRestService();
            var serviceResult = new ServiceResult();
            try
            {
                var token = DataLogic.GetAccessTokenFromSql(GlobalSettings.LogoFirmNumber, GlobalSettings.LogoUserName);
                if (string.IsNullOrEmpty(token))
                {
                    tokenHolder = restService.GetAccessToken();
                    DataLogic.InsertAccessToken(new ErpTokens
                    {
                        UserName = GlobalSettings.LogoUserName,
                        FirmNo = GlobalSettings.LogoFirmNumber.ToInt(),
                        AccessToken = tokenHolder.Token
                    });
                }
                else
                {
                    tokenHolder.IsLoggedIn = true;
                }
                if (tokenHolder.IsLoggedIn)
                {
                    serviceResult.JResult = restService.HttpPost(dataType, jsonData, tokenHolder.Token);
                    if (serviceResult.JResult.HasValues)
                    {
                        if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                        {
                            serviceResult.Success = true;
                            serviceResult.LogRef = serviceResult.JResult["INTERNAL_REFERENCE"].ToObject<int>();
                            serviceResult.ObjectNo = DataLogic.GetFicheNumberOrCardCodeByRef(serviceResult.LogRef, dataType);
                            if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                            {

                            }
                        }
                    }
                    else
                    {
                        bool controlToken = serviceResult.JResult.ToString().Contains("Token") || serviceResult.JResult.ToString().Contains("Authorization") || serviceResult.JResult.ToString().Contains("Unauthorised");
                        if (controlToken)
                        {
                            if (tokenRepeatCount == 0)
                            {
                                DataLogic.UpdateAccessTokenExpriedDate(new ErpTokens
                                { UserName = GlobalSettings.LogoUserName, FirmNo = GlobalSettings.LogoFirmNumber.ToInt() });
                                tokenRepeatCount = 1;
                                CreateData(postId, dataType, jsonData);
                            }
                            else
                            {
                                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), "Token alınamıyor...", serviceResult.JResult));
                            }
                        }
                        else
                        {
                            LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), "Post edilemiyor...", serviceResult.JResult));
                        }
                        DataLogic.InsertResponseLog(new ResponseLog
                        {
                            PostId = postId,
                            PostDate = DateTime.Now,
                            OperationType = dataType,
                            JsonData = jsonData
                        }, ref exception);

                    }

                    DataLogic.InsertResponseLog(new ResponseLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = TigerDataType.BankSlips.Value,
                        IdentityName = "",
                        OperationType = "REQUEST",
                        ResponseData = serviceResult.JResult?.ToString(),
                        JsonData = jsonData
                    }, ref exception);
                }
                else
                {
                    DataLogic.InsertErrorLog(new ErrorLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = dataType,
                        IdentityName = "",
                        OperationType = "POST",
                        ErrorClassName = nameof(RestServiceManager),
                        ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                        ErrorMessage = tokenHolder?.Token,
                        ResponseData = tokenHolder?.Token,
                        JsonData = jsonData
                    }, ref exception);
                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), " ",e.Message));
                DataLogic.InsertErrorLog(new ErrorLog
                {
                    PostId = postId,
                    PostDate = DateTime.Now,
                    HostIp = dataType,
                    IdentityName = "",
                    OperationType = "POST",
                    ErrorClassName = nameof(RestServiceManager),
                    ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                    ErrorMessage = e.Message,
                    ResponseData = tokenHolder?.Token,
                    JsonData = jsonData
                }, ref exception);
            }

            return serviceResult;
        }

        public ServiceResult CreateGlSlip(GlSlip glSlip, TigerDataType dataType)
        {
            //var request = HttpContext.Current.Request;
            var tokenHolder = TokenHolder.GetInstance();
            var restService = new TigerRestService();
            string json = string.Empty;
            var serviceResult = new ServiceResult();
            try
            {
                //orderFiche.DataObjectParameter = new DataObjectParameter();
                json = JsonConvert.SerializeObject(glSlip, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                postId = DataLogic.InsertPostLog(new PostLog
                {
                    PostDate = DateTime.Now,
                    HostIp = dataType.Value,
                    IdentityName = "",
                    OperationType = "POST",
                    Url = dataType.Value,
                    RequestMethod = "POST",
                    JsonData = json
                }, ref exception).LogRef;
                if (restService.GetAccessToken().IsLoggedIn)
                {
                    serviceResult.JResult = restService.HttpPost(dataType.Value, json);
                    if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                    {
                        serviceResult.Success = true;
                        serviceResult.LogRef = serviceResult.JResult["INTERNAL_REFERENCE"].ToObject<int>();
                        serviceResult.ObjectNo = DataLogic.GetEmFicheNumberByRef(serviceResult.LogRef, ref exception);
                        if (serviceResult.JResult != null && !serviceResult.JResult.ToString().Contains("Message"))
                        {

                        }
                    }
                }
                else
                {
                    DataLogic.InsertErrorLog(new ErrorLog
                    {
                        PostId = postId,
                        PostDate = DateTime.Now,
                        HostIp = dataType.Value,
                        IdentityName = "",
                        OperationType = "POST",
                        ErrorClassName = nameof(ServiceManager),
                        ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                        ErrorMessage = serviceResult.JResult.ToString(),
                        ResponseData = serviceResult.JResult.ToString(),
                        JsonData = json
                    }, ref exception);
                }
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), " ", e.Message));
                DataLogic.InsertErrorLog(new ErrorLog
                {
                    PostId = postId,
                    PostDate = DateTime.Now,
                    HostIp = dataType.Value,
                    IdentityName = "",
                    OperationType = "POST",
                    ErrorClassName = nameof(RestServiceManager),
                    ErrorMethodName = MethodBase.GetCurrentMethod()?.Name,
                    ErrorMessage = e.Message,
                    ResponseData = tokenHolder?.Token,
                    JsonData = json
                }, ref exception);
            }

            return serviceResult;
        }


    }
}
