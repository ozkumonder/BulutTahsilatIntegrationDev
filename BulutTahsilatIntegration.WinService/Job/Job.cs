using System;
using System.Threading.Tasks;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.Model.Global;
using BulutTahsilatIntegration.WinService.ServiceManager;
using BulutTahsilatIntegration.WinService.Utilities;
using Quartz;

namespace BulutTahsilatIntegration.WinService.Job
{
    public class Job : IJob
    {
        private ConfigSettings Settings;
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //ClientServiceManager.GetBankBalance();
                //var begDate = new DateTime(2021, 12, 31, 0, 0, 0);
                //var endDate = new DateTime(2022, 1, 1, 3, 59, 59);
                //var begDate = DateTime.Now.AddDays(-7);//new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                //var startDate = new DateTime(2022, 1, 1);
                //begDate = DateTime.Now.AddDays(-7).Date;
                //if (begDate < startDate)
                //{
                //    begDate = DateTime.Now;
                //}
                foreach (var firm in GlobalSettings.Firms.Firm)
                {
                    GlobalSettings.Firm = firm;
                    var begDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-7).Day);
                    var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    ClientServiceManager.SendingTransferSendLogo(begDate, endDate,firm.TaxNumber);
                    ClientServiceManager.IncommingTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.BankTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.CheckTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.BankServiceInvoiceTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.CreditPaySendingTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.LeasingPaySendingTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.BuyExchangeBankTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.SalesExchangeBankTransferSendLogo(begDate, endDate, firm.TaxNumber);
                    ClientServiceManager.ArbitrajBankTransferSendLogo(begDate, endDate, firm.TaxNumber);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, nameof(Job), StringUtil.Seperator, "Execute", StringUtil.Seperator, exception.Message, StringUtil.Seperator, exception.InnerException));
            }
            finally
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), string.Concat(" <---Finish Job Erp Operation Executing--->", Environment.NewLine)));
            }
        }
    }
}