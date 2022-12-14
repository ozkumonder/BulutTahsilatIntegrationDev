using System;
using System.IO;

namespace BulutTahsilatIntegration.WinService.Core
{
    public static class LogHelper
    {
        public static void Log(string stringText)
        {
            try
            {
                if (string.IsNullOrEmpty(stringText)) return;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                directoryName = string.Concat(directoryName, "\\Logs");
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                DateTime now = DateTime.Now;
                string str = string.Concat(directoryName, "\\Log_", now.ToString("dd-MM-yyyy"), ".txt");
                using (var streamWriter = new StreamWriter(str, true))
                {
                    streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            catch (Exception)
            {

            }

        }
        public static void LogEx(string stringText)
        {
            try
            {
                if (string.IsNullOrEmpty(stringText)) return;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                directoryName = string.Concat(directoryName, "\\Logs");
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                DateTime now = DateTime.Now;
                string str = string.Concat(directoryName, "\\Log_WareHouse_", now.ToString("dd-MM-yyyy"), ".txt");
                using (var streamWriter = new StreamWriter(str, true))
                {
                    streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            catch (Exception)
            {

            }

        }
        public static void LogError(string stringText)
        {
            try
            {
                if (string.IsNullOrEmpty(stringText)) return;
                //StreamWriter streamWriter;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                directoryName = string.Concat(directoryName, "\\Logs");
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                DateTime now = DateTime.Now;
                string str = string.Concat(directoryName, "\\ErrorLog_", now.ToString("dd-MM-yyyy"), ".txt");
                using (var streamWriter = new StreamWriter(str, true))
                {
                    streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
                    streamWriter.Close();
                    streamWriter.Dispose();
                }


            }
            catch (Exception)
            {

            }

        }
        public static void LogDbError(string stringText)
        {
            try
            {
                if (string.IsNullOrEmpty(stringText)) return;
                //StreamWriter streamWriter;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                directoryName = string.Concat(directoryName, "\\Logs");
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                DateTime now = DateTime.Now;
                string str = string.Concat(directoryName, "\\DatabaseOperationErrorLog_", now.ToString("dd-MM-yyyy"), ".txt");
                using (var streamWriter = new StreamWriter(str, true))
                {
                    streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
                    streamWriter.Close();
                    streamWriter.Dispose();
                }


            }
            catch (Exception)
            {

            }

        }
        public static void LogErrorEx(string stringText)
        {
            try
            {
                if (string.IsNullOrEmpty(stringText)) return;
                //StreamWriter streamWriter;
                string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                directoryName = string.Concat(directoryName, "\\Logs");
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                DateTime now = DateTime.Now;
                string str = string.Concat(directoryName, "\\ErrorLog_WareHouse_", now.ToString("dd-MM-yyyy"), ".txt");
                using (var streamWriter = new StreamWriter(str, true))
                {
                    streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
                    streamWriter.Close();
                    streamWriter.Dispose();
                }


            }
            catch (Exception)
            {

            }

        }
        public static string ToLogType(this LogType type)
        {
            string logType = string.Empty;
            switch (type)
            {
                case LogType.Application:
                    logType = "[Application]: ";
                    break;
                case LogType.Error:
                    logType = "[ERROR]: ";
                    break;
                case LogType.Warning:
                    logType = "[WARN]: ";
                    break;
                case LogType.Info:
                    logType = "[INFO]: ";
                    break;
                case LogType.Question:
                    logType = "[INFO]: ";
                    break;
            }
            return logType;
        }
        public enum LogType
        {
            Application,
            Error,
            Warning,
            Info,
            Question
        }
    }
}
