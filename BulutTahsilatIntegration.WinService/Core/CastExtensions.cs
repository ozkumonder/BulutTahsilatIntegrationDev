using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.Core
{

    public static class CastExtensions
    {
        public static dynamic Cast<T>(this T from, dynamic to)
        {
            PropertyInfo[] properties = from.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                to.GetType().GetProperty(propertyInfo.Name).SetValue(to, propertyInfo.GetValue(from, null));
            }
            return to;
        }
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (chunksize <= 0)
            {
                throw new ArgumentException("Chunk size must be greater than zero.", "chunksize");
            }
            while (source.Any<T>())
            {
                yield return source.Take<T>(chunksize);
                source = source.Skip<T>(chunksize);
            }
        }

        public static bool IsDecimal(this object value)
        {
            bool flag;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                Convert.ToDecimal(value); flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsDouble(this object value)
        {
            bool flag;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                Convert.ToDouble(value);
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsInteger(this object value)
        {
            bool flag;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                Convert.ToInt32(value);
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return (!string.IsNullOrWhiteSpace(str) ? false : true);
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string SubstringByLeft(this string metin, int uzunluk)
        {
            string str;
            str = (metin.Length >= uzunluk ? string.Concat(metin.Substring(0, uzunluk), "..") : metin);
            return str;
        }

        public static bool ToBool(this object s)
        {
            bool flag;
            try
            {
                flag = Convert.ToBoolean(s);
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        public static byte ToByte(this object value)
        {
            byte num;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                num = Convert.ToByte(value);
            }
            catch (Exception)
            {
                num = 0;
            }
            return num;
        }
        public static short ToShort(this object value)
        {
            short ınt16;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                ınt16 = Convert.ToInt16(value);
            }
            catch (Exception)
            {
                ınt16 = 0;
            }
            return ınt16;
        }

        public static int ToInt(this object value)
        {
            int ınt32;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                ınt32 = Convert.ToInt32(value);
            }
            catch (Exception)
            {
                ınt32 = 0;
            }
            return ınt32;
        }
        public static decimal ToDecimal(this object value)
        {
            decimal num;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                num = Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                num = new decimal(0);
            }
            return num;
        }

        public static double ToDouble(this object value)
        {
            double num;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                num = Convert.ToDouble(value);
            }
            catch (Exception)
            {
                num = 0;
            }
            return num;
        }

        public static long ToLong(this object value)
        {
            long ınt64;
            try
            {
                if (value == null)
                {
                    throw new Exception();
                }
                ınt64 = Convert.ToInt64(value);
            }
            catch (Exception)
            {
                ınt64 = (long)0;
            }
            return ınt64;
        }

        public static DateTime ToDateTime(this object s)
        {
            DateTime dateTime;
            try
            {
                dateTime = Convert.ToDateTime(s);
            }
            catch (Exception)
            {
                dateTime = DateTime.MinValue;
            }
            return dateTime;
        }

        public static string ToDbShortString(this DateTime dateTime)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            object[] objArray = new object[] { dateTime };
            return string.Format(culture, "{0:MM/dd/yyyy}", objArray);
        }
        public static DateTime ToSmallDateTime(this object value)
        {
            DateTime dateTime;
            try
            {
                dateTime = Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                dateTime = new DateTime(1960, 1, 1);
            }
            return dateTime;
        }


        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expandoObjects = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(anonymousObject))
            {
                object value = property.GetValue(anonymousObject);
                expandoObjects.Add(property.Name, value);
            }
            return (ExpandoObject)expandoObjects;
        }
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }




        public static byte[] GetBytes(this object obj)
        {
            byte[] array;
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (!(obj is Stream))
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        bf.Serialize(ms, obj);
                    }
                    catch (SerializationException serializationException)
                    {
                        SerializationException ex = serializationException;
                        //throw new CoreWorkCommonException(CoreWorkExceptionTypes.InvalidOperation, CoreWorkExceptionMessages.InvalidArgument, ex, new object[] { "Argument must be serializable." });
                    }
                    array = ms.ToArray();
                }
            }
            else
            {
                byte[] buffer = new byte[16384];
                using (MemoryStream ms = new MemoryStream())
                {
                    while (true)
                    {
                        int ınt32 = ((Stream)obj).Read(buffer, 0, (int)buffer.Length);
                        int read = ınt32;
                        if (ınt32 <= 0)
                        {
                            break;
                        }
                        ms.Write(buffer, 0, read);
                    }
                    array = ms.ToArray();
                }
            }
            return array;
        }
        public static T ConvertTo<T>(this object value, T defaultValue, IFormatProvider provider)
        {
            T t;
            Type type = typeof(T);
            Type argType = type;
            if ((!type.IsGenericType ? false : type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                argType = type.GetGenericArguments().First<Type>();
            }
            try
            {
                t = (T)Convert.ChangeType(value, argType, provider);
            }
            catch (Exception)
            {
                t = defaultValue;
            }
            return t;
        }
        public static T ConvertTo<T>(object value)
        {
            return ConvertTo<T>(value, default(T));
        }

        public static T ConvertTo<T>(object value, T defaultValue)
        {
            return ConvertTo<T>(value, default(T), CultureInfo.CurrentCulture);
        }
        public static T As<T>(this object value)
        {
            return ConvertTo<T>(value);
        }

        public static string ToTotalString(this double total)
        {
            string sTutar = total.ToString("F2").Replace('.', ','); // Replace('.',',') ondalık ayracının . olma durumu için            
            string lira = sTutar.Substring(0, sTutar.IndexOf(',')); //tutarın tam kısmı
            string kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
            string yazi = "";

            string[] birler = { "", "BİR", "İKİ", "Üç", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };
            string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
            string[] binler = { "KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", "" }; //KATRİLYON'un önüne ekleme yapılarak artırabilir.

            int grupvaluesi = 6; //sayıdaki 3'lü grup sayısı. katrilyon içi 6. (1.234,00 daki grup sayısı 2'dir.)
                                 //KATRİLYON'un başına ekleyeceğiniz her değer için grup sayısını artırınız.

            lira = lira.PadLeft(grupvaluesi * 3, '0'); //sayının soluna '0' eklenerek sayı 'grup sayısı x 3' basakmaklı yapılıyor.            

            string grupDegeri;

            for (int i = 0; i < grupvaluesi * 3; i += 3) //sayı 3'erli gruplar halinde ele alınıyor.
            {
                grupDegeri = "";

                if (lira.Substring(i, 1) != "0")
                    grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ"; //yüzler                

                if (grupDegeri == "BİRYÜZ") //biryüz düzeltiliyor.
                    grupDegeri = "YÜZ";

                grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))]; //onlar

                grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))]; //birler                

                if (grupDegeri != "") //binler
                    grupDegeri += binler[i / 3];

                if (grupDegeri == "BİRBİN") //birbin düzeltiliyor.
                    grupDegeri = "BİN";

                yazi += grupDegeri;
            }

            if (yazi != "")
                yazi += " TL ";

            int? yaziUzunlugu = yazi.Length;

            if (kurus.Substring(0, 1) != "0") //kuruş onlar
                yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];

            if (kurus.Substring(1, 1) != "0") //kuruş birler
                yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];

            if (yazi.Length > yaziUzunlugu)
                yazi += " Kr.";
            else
                yazi += "SIFIR Kr.";

            return yazi;
        }

    }

}