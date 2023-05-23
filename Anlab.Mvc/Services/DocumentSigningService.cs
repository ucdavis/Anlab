using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Configuration;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Services;

public interface IDocumentSigningService
{
    Task<string> SendForEmbeddedSigning(int orderId);
}

public class DocumentSigningService : IDocumentSigningService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ESignatureOptions _eSignatureSettings;

    private static readonly string SignerClientId = "1000";

    public DocumentSigningService(IOptions<ESignatureOptions> eSignatureSettings, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _eSignatureSettings = eSignatureSettings.Value;
    }

    // TODO: need to pull in content based on the order
    public async Task<string> SendForEmbeddedSigning(int orderId)
    {
        var order = await _dbContext.Orders.Include(o=>o.Creator).SingleAsync(a => a.Id == orderId);

        if (order == null)
        {
            throw new ArgumentException("Order not found");
        }

        // first, get auth token
        var authToken = AuthenticateWithJwt();

        var user = GetAccountInfo(authToken);

        var account = user.Accounts[0]; // TODO: what if there are multiple accounts?  need to select one somehow perhaps

        var acctId = account.AccountId;
        var basePath = account.BaseUri;

        // next, make the envelope
        var envelope = await MakeEnvelope(order);

        // now create the envelope via api
        var client = new DocuSignClient(basePath + "/restapi");
        client.Configuration.DefaultHeader.Add("Authorization", "Bearer " + authToken.access_token);

        var envelopesApi = new EnvelopesApi(client);

        var results = await envelopesApi.CreateEnvelopeAsync(acctId, envelope);

        // finally, redirect to the signing url
        var viewRequest = new RecipientViewRequest
        {
            ReturnUrl = _eSignatureSettings.ApplicationBaseUri + "/order/SignatureCallback/" + orderId,
            ClientUserId = SignerClientId,
            AuthenticationMethod = "none",
            UserName = order.Creator.Name,
            Email = order.Creator.Email
        };

        var view = await envelopesApi.CreateRecipientViewAsync(acctId, results.EnvelopeId, viewRequest);

        var url = view.Url; // url to redirect to -- can save state via query params on return url if needed

        return url;
    }

    private OAuth.UserInfo GetAccountInfo(OAuth.OAuthToken authToken)
    {
        var client = new DocuSignClient();

        client.SetOAuthBasePath(_eSignatureSettings.AuthServer);

        var userInfo = client.GetUserInfo(authToken.access_token);

        return userInfo;
    }

    private async Task<EnvelopeDefinition> MakeEnvelope(Order order)
    {
        Debug.Assert(_httpContextAccessor.HttpContext != null, "_httpContextAccessor.HttpContext != null");

        var cookies = _httpContextAccessor.HttpContext.Request.Cookies;

        // add html doc and add signer tag to it
        var env = new EnvelopeDefinition
        {
            EmailSubject = "Please sign this document"
        };

        // read html from url /order/document/{id}
        using var handler = new HttpClientHandler { UseCookies = false };
        using var client = new HttpClient(handler);
        client.BaseAddress = new Uri(_eSignatureSettings.ApplicationBaseUri);

        // add all cookies to the request
        foreach (var cookie in cookies)
        {
            client.DefaultRequestHeaders.Add("Cookie", cookie.Key + "=" + cookie.Value);
        }

        var html = await client.GetStringAsync("/order/document/" + order.Id);

        // add a document to the envelope
        var doc = new Document
        {
            DocumentBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(html)),
            Name = "Order acknowledgement",
            FileExtension = "html",
            DocumentId = "1"
        };

        env.Documents = new List<Document> {doc};

        // add a signer to the envelope
        var signer = new Signer
        {
            Email = order.Creator.Email,
            Name = order.Creator.Name,
            ClientUserId = SignerClientId,
            RecipientId = "1",
        };

        // create sign here tab (signing field on document) based off of anchor string
        var signHere = new SignHere
        {
            AnchorString = "**signature_1**",
            AnchorUnits = "pixels",
            AnchorYOffset = "10",
            AnchorXOffset = "20",
        };

        // add sign here tab to signer
        signer.Tabs = new Tabs {SignHereTabs = new List<SignHere> {signHere}};

        // add signer to envelope
        env.Recipients = new Recipients {Signers = new List<Signer> {signer}};

        // set envelope status to sent
        env.Status = "sent";

        return env;
    }

    private OAuth.OAuthToken AuthenticateWithJwt()
    {
        var docuSignClient = new DocuSignClient();

        var scopes = new List<string>
        {
            OAuth.Scope_SIGNATURE,
            OAuth.Scope_IMPERSONATION
        };

        // get the private key from base64 encoded string
        var privateKey = Convert.FromBase64String(_eSignatureSettings.PrivateKeyBase64);

        return docuSignClient.RequestJWTUserToken(
            _eSignatureSettings.ClientId,
            _eSignatureSettings.ImpersonatedUserId,
            _eSignatureSettings.AuthServer,
            privateKey,
            1,
            scopes);
    }
}
