using System;
using System.Runtime.Serialization;

namespace AdobeSign.Contracts
{
    [DataContract]
    public class AccessToken
    {
        [DataMember(EmitDefaultValue = false)]
        public string token_type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string access_token { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string refresh_token { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int expires_in { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? TokenExpireDate { get; set; }
    }
}
