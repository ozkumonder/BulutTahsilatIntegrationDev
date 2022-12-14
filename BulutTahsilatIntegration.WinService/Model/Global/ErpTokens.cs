using Newtonsoft.Json;

namespace BulutTahsilatIntegration.WinService.Model.Global
{
    public class ErpTokens
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("as:client_id")]
        public string AsClientId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("firmNo")]
        public long FirmNo { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("dbName")]
        public string DbName { get; set; }

        [JsonProperty("logoDB")]
        public string LogoDb { get; set; }

        [JsonProperty("isLoginEx")]
        public string IsLoginEx { get; set; }

        [JsonProperty("isLogoPlugin")]
        public string IsLogoPlugin { get; set; }

        [JsonProperty("idmToken")]
        public string IdmToken { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }
    }
}
