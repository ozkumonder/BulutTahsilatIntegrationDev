using System;

namespace BulutTahsilatIntegration.WinService.BTIIntegration
{
    public class TokenHolder
    {
        private static TokenHolder instance;

        public int ExpireSeconds
        {
            get;
            set;
        }

        public bool IsValid
        {
            get
            {
                return DateTimeOffset.Now < this.ValidUntil;
            }
        }

        public bool IsLoggedIn { get;set; }
        public string Token
        {
            get;
            set;
        }

        public DateTimeOffset ValidUntil
        {
            get;
            set;
        }

        static TokenHolder()
        {
            TokenHolder.instance = new TokenHolder();
        }

        private TokenHolder()
        {
        }

        public static TokenHolder GetInstance()
        {
            return TokenHolder.instance;
        }
    }
}