namespace AnlabMvc.Models.Configuration;

public class ESignatureOptions
{
    public string ClientId { get; set; }
    public string ImpersonatedUserId { get; set; }
    public string AuthServer { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string PrivateKeyBase64 { get; set; }

}
