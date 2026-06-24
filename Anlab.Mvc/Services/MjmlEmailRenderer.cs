using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Mjml.Net;

namespace AnlabMvc.Services
{
    public interface IMjmlEmailRenderer
    {
        Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default);
    }

    public class MjmlEmailRenderer : IMjmlEmailRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly MjmlRenderer _mjmlRenderer;
        private readonly ILogger<MjmlEmailRenderer> _logger;

        public MjmlEmailRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            MjmlRenderer mjmlRenderer,
            ILogger<MjmlEmailRenderer> logger)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _mjmlRenderer = mjmlRenderer;
            _logger = logger;
        }

        public async Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentException("Template name is required.", nameof(templateName));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var mjmlMarkup = await RenderRazorViewAsync(templateName, model);

            cancellationToken.ThrowIfCancellationRequested();

            var (html, errors) = _mjmlRenderer.Render(mjmlMarkup, new MjmlOptions
            {
                Beautify = false
            });

            if (errors.Count > 0)
            {
                var errorMessage = string.Join(Environment.NewLine, errors.Select(error => error.ToString()));
                _logger.LogError("MJML rendering failed for template {TemplateName}: {Errors}", templateName, errorMessage);
                throw new InvalidOperationException($"Failed to render MJML email template '{templateName}': {errorMessage}");
            }

            return html;
        }

        private async Task<string> RenderRazorViewAsync<TModel>(string templateName, TModel model)
        {
            var actionContext = GetDefaultActionContext();
            var viewEngineResult = _viewEngine.FindView(actionContext, templateName, false);

            if (!viewEngineResult.Success)
            {
                viewEngineResult = _viewEngine.GetView(null, templateName, false);
            }

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find MJML email view '{templateName}'.");
            }

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewEngineResult.View,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await viewEngineResult.View.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private ActionContext GetDefaultActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
