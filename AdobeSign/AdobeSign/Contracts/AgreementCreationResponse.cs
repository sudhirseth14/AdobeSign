using System.Runtime.Serialization;

namespace AdobeSign.Contracts
{
    [DataContract]
    public class AgreementCreationResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public string id { get; set; }
    }
}
