using System.Runtime.Serialization;

namespace AdobeSign.Contracts
{
    [DataContract]
    public class TransientDocumentResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public string transientDocumentId { get; set; }
    }
}
