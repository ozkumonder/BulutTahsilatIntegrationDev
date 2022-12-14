using System;
using System.Reflection;
using System.Threading.Tasks;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.Model.Global;
using BulutTahsilatIntegration.WinService.ServiceManager;
using BulutTahsilatIntegration.WinService.Utilities;
using Quartz;

namespace BulutTahsilatIntegration.WinService.Job
{
    public class JobClient : IJob
    {
        private ConfigSettings Settings;
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Execute();
            }
            catch (System.Exception exception)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, nameof(JobClient), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name, StringUtil.Seperator, exception.Message));
            }
            finally
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), string.Concat(" <---Finish JobClient Executing--->", Environment.NewLine)));
            }
        }
        public void Execute()
        {
            try
            {
                foreach (var firm in GlobalSettings.Firms.Firm)
                {
                    GlobalSettings.Firm = firm;
                    var begDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    var endDate = DateTime.Now;//new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day, 23, 59, 59);
                    ClientServiceManager.SendClient();
                    ClientServiceManager.SendServiceCard();
                }

                
            }
            catch (System.Exception exception)
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Error.ToLogType(), StringUtil.Seperator, nameof(JobClient), StringUtil.Seperator, MethodBase.GetCurrentMethod().Name, StringUtil.Seperator, exception.Message));
            }
            finally
            {
                LogHelper.Log(string.Concat(LogHelper.LogType.Info.ToLogType(), string.Concat(" <---Finish JobClient Executing--->", Environment.NewLine)));
            }
        }
    }
}