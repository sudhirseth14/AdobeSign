
namespace AdobeSign.Models
{
    public class ApplicationDetails
    {
        public ApplicationDetails(
            string apiurl,
            string client_id,
            string client_secret,
            string redirect_url)
        {

            this.APIUrl = apiurl;
            this.ClientID = client_id;
            this.ClientSecret = client_secret;
            this.RedirectUrl = redirect_url;

        }
        public string APIUrl { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
    }
}
