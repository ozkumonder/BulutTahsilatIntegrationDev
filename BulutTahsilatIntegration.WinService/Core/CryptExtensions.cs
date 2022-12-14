using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BulutTahsilatIntegration.WinService.Core
{
    public static class CryptExtensions
    {
        private static DESCryptoServiceProvider desProvider = null;
        private static byte[] des_Key;
        private static byte[] des_IV;

        private static readonly byte[] _rgbKey;
        private static readonly byte[] _rgbIv;

        static CryptExtensions()
        {
            
        }

        public static string DecryptIt(this string toDecrypt)
        {
            return "";
        }

        public static string EncryptIt(this string toEnrypt)
        {
            return "";
        }

        #region DES

        /// <summary>
        /// DES Algoritması Şifreleme Methodu algorithm 
        /// </summary>
        /// <param name="source">Şifrelenecek string parametre.</param>
        /// <returns>Şifrelenmiş string döner.</returns>
        public static string EncryptIt_Des(this string source)
        {

            return Convert.ToBase64String(Encypt<DESCryptoServiceProvider>(desProvider, source, des_Key, des_IV));
        }

        /// <summary>
        /// Des Algoritması Şifre Çözme Methodu
        /// </summary>
        /// <param name="source">Şifre çözümü olacak parametre</param>
        /// <returns>Şifresiz string döner.</returns>
        public static string DecryptIt_Des(this string source)
        {
            return Decrypt<DESCryptoServiceProvider>(desProvider, x, des_Key, des_IV);
        }

        #endregion

        #region Generic şifreleme ve çözümleme metodları
        /// <summary>
        /// Generic Şifreleme Methodu
        /// </summary>
        /// <typeparam name="T">Algoritma Sağlayıcı Sınıf <c>DESCryptoServiceProvider<c></typeparam>
        /// <param name="provider"></param>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        static byte[] Encypt<T>(T provider, string data, byte[] key, byte[] iv) where T : SymmetricAlgorithm
        {
            byte[] result = null;
            return result;
        }
        /// <summary>
        /// Generic Şifre Çözme Methodu
        /// </summary>
        /// <typeparam name="T">Algoritma Sağlayıcı Sınıf <c>DESCryptoServiceProvider<c></typeparam>
        /// <param name="provider"></param>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        static string Decrypt<T>(T provider, byte[] source, byte[] key, byte[] iv) where T : SymmetricAlgorithm
        {
            string result = string.Empty;

            
            return result;
        }

        #endregion




    }
}
