using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Services;

public interface IDocumentSigningService
{
    Task<string> SendForEmbeddedSigning(int orderId);
    Task<FileStreamResult> DownloadEnvelope(string envelopeId);
    OAuth.UserInfo GetAccountInfo(OAuth.OAuthToken authToken);
    OAuth.OAuthToken AuthenticateWithJwt();
    string BuildConsentUrl();
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

        var account = user.Accounts.Single(a => a.IsDefault.Equals("true", StringComparison.OrdinalIgnoreCase)); 

        var acctId = account.AccountId;
        var basePath = account.BaseUri;

        // next, make the envelope
        var envelope = await MakeEnvelope(order);

        // now create the envelope via api
        var client = new DocuSignClient(basePath + "/restapi");
        client.Configuration.DefaultHeader.Add("Authorization", "Bearer " + authToken.access_token);

        var envelopesApi = new EnvelopesApi(client);

        var results = await envelopesApi.CreateEnvelopeAsync(acctId, envelope);

        var returnUrl = _eSignatureSettings.ApplicationBaseUri + "/order/SignatureCallback/" + orderId + "?envelopeId=" + results.EnvelopeId;

        // finally, redirect to the signing url
        var viewRequest = new RecipientViewRequest
        {
            ReturnUrl = returnUrl,
            ClientUserId = SignerClientId,
            AuthenticationMethod = "none",
            UserName = order.Creator.Name,
            Email = order.Creator.Email
        };

        var view = await envelopesApi.CreateRecipientViewAsync(acctId, results.EnvelopeId, viewRequest);

        var url = view.Url; // url to redirect to -- can save state via query params on return url if needed

        return url;
    }

    public async Task<FileStreamResult> DownloadEnvelope(string envelopeId)
    {
        // first, get auth token
        var authToken = AuthenticateWithJwt();

        var user = GetAccountInfo(authToken);

        // there should always be only one default account
        var account = user.Accounts.First(a => a.IsDefault == "true");

        var acctId = account.AccountId;
        var basePath = account.BaseUri;

        // now create the envelope via api
        var client = new DocuSignClient(basePath + "/restapi");
        client.Configuration.DefaultHeader.Add("Authorization", "Bearer " + authToken.access_token);

        var envelopesApi = new EnvelopesApi(client);

        // "combined" is the type of document to download.  Other options include "archive" and "certificate"
        var results = await envelopesApi.GetDocumentAsync(acctId, envelopeId, "combined");

        return new FileStreamResult(results, "application/pdf");
    }

    public OAuth.UserInfo GetAccountInfo(OAuth.OAuthToken authToken)
    {
        var client = new DocuSignClient();

        client.SetOAuthBasePath(_eSignatureSettings.AuthServer);

        var userInfo = client.GetUserInfo(authToken.access_token);

        return userInfo;
    }

    public OAuth.OAuthToken AuthenticateWithJwt()
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

    public string BuildConsentUrl()
    {
        var scopes = "signature impersonation";

        Debug.Assert(_httpContextAccessor.HttpContext != null, "_httpContextAccessor.HttpContext != null");

        var path = new Uri(_httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host);

        return _eSignatureSettings.AuthorizationEndpoint + "?response_type=code" +
               "&scope=" + scopes +
               "&client_id=" + _eSignatureSettings.ClientId +
               "&redirect_uri=" + path + "admin";
    }

    private async Task<EnvelopeDefinition> MakeEnvelope(Order order)
    {
        Debug.Assert(_httpContextAccessor.HttpContext != null, "_httpContextAccessor.HttpContext != null");

        var cookies = _httpContextAccessor.HttpContext.Request.Cookies;

        // add html doc and add signer tag to it
        var env = new EnvelopeDefinition
        {
            EmailSubject = "Please sign this document",
            // the document should expire after 2 days with no reminder
            Notification = new Notification
            {
                UseAccountDefaults = "false",
                Reminders = new Reminders
                {
                    ReminderEnabled = "false",
                },
                Expirations = new Expirations
                {
                    ExpireEnabled = "true",
                    ExpireAfter = "2",
                    ExpireWarn = "0"
                }
            }
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
            // AnchorYOffset = "10",
            AnchorXOffset = "20",
        };

        var dateSigned = new DateSigned
        {
            AnchorString = "**date_1**",
            AnchorUnits = "pixels",
            // AnchorYOffset = "10",
            AnchorXOffset = "20",
        };

        // add sign here & date signed tabs for signer
        signer.Tabs = new Tabs {SignHereTabs = new List<SignHere> {signHere}, DateSignedTabs = new List<DateSigned> {dateSigned }};

        // add signer to envelope
        env.Recipients = new Recipients {Signers = new List<Signer> {signer}};

        // set envelope status to sent
        env.Status = "sent";

        return env;
    }
}
