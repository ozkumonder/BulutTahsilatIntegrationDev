using System;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.Model.Global;

namespace BulutTahsilatIntegration.WinService.Utilities
{
    public static class ExtensionMethodsForLogo
    {
        /// <summary>
        /// Verilen tarih parametresini logo formatında long olarak döner
        /// </summary>
        /// <param name="date">Tarih bilgisi verilir</param>
        /// <returns></returns>
        public static long LogoPackDate(this object date)
        {
            long year;
            try
            {
                if (date == null)
                {
                    throw new Exception();
                }
                DateTime dateTime = Convert.ToDateTime(date);
                year = (long)(dateTime.Year * 65536 + dateTime.Month * 256 + dateTime.Day);
            }
            catch (Exception)
            {
                year = 0;
            }
            return year;
        }
        /// <summary>
        /// Verilen zaman parametresini logo formatında long olarak döner
        /// </summary>
        /// <param name="time">Zaman bilgilisi verilir</param>
        /// <returns></returns>
        public static long LogoPackTime(this object time)
        {
            long hour;
            try
            {
                if (time == null)
                {
                    throw new Exception();
                }
                DateTime dateTime = Convert.ToDateTime(time);
                hour = (long)(dateTime.Hour * 65536 * 256 + dateTime.Minute * 65536 + dateTime.Second * 256);
            }
            catch (Exception)
            {
                hour = (long)0;
            }
            return hour;
        }

        public static string QuotedStr(this string text)
        {
            return string.Concat("'", text, "'");
        }

        /// <summary>
        /// Sorgu içerisinde kullanulan XXX olarak tanımlanan firma ve dönem bilgilirini tanımlanmış firma dönem bilgileriyle değiştirir
        /// </summary>
        /// <param name="query">SQL query verilir</param>
        /// <returns></returns>
        public static string ToReplaceLogoTableName(this string query)
        {
            var logoFirmNR = GlobalSettings.Firm.FirmNumber;
            var logoPeriodNR = "01";
            if (logoFirmNR == "0")
            {
                string str1 = "00";
                return str1;
            }
            else
            {
                string str = query.Replace("XXX", logoFirmNR.ToString().PadLeft(3, '0'));
                string str1 = str.Replace("XX", logoPeriodNR.ToString().PadLeft(2, '0'));
                query = str1;
                return str1;
            }
            //return "";}
        }
        public static double ToRoundUp2Decimal(this object sayi)
        {
            double num;
            try
            {
                if (sayi == null)
                {
                    throw new Exception();
                }
                num = Math.Round(Convert.ToDouble(sayi), 2);
            }
            catch (Exception)
            {
                num = 0;
            }
            return num;
        }
    }
}
