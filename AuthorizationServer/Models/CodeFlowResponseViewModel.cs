using System.Text.Json.Serialization;

namespace OIDCDemo.AuthorizationServer.Models
{
    public class CodeFlowResponseViewModel : CodeFlowResponseModel
    {
        [JsonPropertyName("redirect_uri")]
        public required string RedirectUri { get; set; }
    }
}
