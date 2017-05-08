using System;
using System.IO;
using AnlabMvc.Models.Pages;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace AnlabMvc.Controllers
{
    public class PagesController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IFileProvider _fileProvider;
        private readonly string _cacheKey;

        public PagesController(IHostingEnvironment hostingEnvironment, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _fileProvider = new PhysicalFileProvider(hostingEnvironment.ContentRootPath + "/pages");

            _cacheKey = "PagesController_ViewPage_";
        }

        public IActionResult ViewPage(string id)
        {
            try
            {
                MarkdownPage model;

                // fetch file info
                var info = _fileProvider.GetFileInfo($"{id}.md");
                if (!info.Exists)
                {
                    return NotFound();
                }

                if (info.IsDirectory)
                {
                    return NotFound();
                }

                // Look for cache key, check last modified date.
                if (_cache.TryGetValue(_cacheKey + id, out model) && model.LastModified >= info.LastModified)
                {
                    return View(model);
                }

                // read out file and parse it
                using (var fs = info.CreateReadStream())
                using (var sr = new StreamReader(fs))
                {
                    var source = sr.ReadToEnd();
                    var html = Markdown.ToHtml(source);

                    model = new MarkdownPage()
                    {
                        Html = html,
                        LastModified = info.LastModified
                    };

                    // cache for up to 15 minutes
                    var options = new MemoryCacheEntryOptions().SetSlidingExpiration(new TimeSpan(0, 15, 0));
                    _cache.Set(_cacheKey + id, model, options);
                }

                return View(model);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }

        }
    }
}