using System;
using System.IO;
using AnlabMvc.Models.Pages;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace AnlabMvc.Controllers
{
    public class PagesController : Controller
    {
        private readonly IFileProvider _fileProvider;

        public PagesController(IHostingEnvironment hostingEnvironment)
        {
            _fileProvider = new PhysicalFileProvider(hostingEnvironment.ContentRootPath + "/pages");
        }

        public IActionResult ViewPage(string id)
        {
            try
            {
                var info = _fileProvider.GetFileInfo($"{id}.md");
                if (!info.Exists)
                {
                    return NotFound();
                }

                if (info.IsDirectory)
                {
                    return NotFound();
                }

                using (var fs = info.CreateReadStream())
                using (var sr = new StreamReader(fs))
                {
                    var source = sr.ReadToEnd();
                    var html = Markdown.ToHtml(source);

                    var model = new MarkdownPage()
                    {
                        Html = html
                    };

                    return View(model);
                }
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }

        }
    }
}