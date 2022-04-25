using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Caching.Memory;

namespace Anlab.Mvc.Views.Shared.Components.DynamicScripts
{
    [ViewComponent(Name = "DynamicScripts")]
    public class DynamicScripts : ViewComponent
    {
        private readonly IFileProvider _fileProvider;
        private readonly IMemoryCache _memoryCache;

        public DynamicScripts(IFileProvider fileProvider, IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._fileProvider = fileProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get the CRA generated index file, which includes optimized scripts
            var indexPage = _fileProvider.GetFileInfo("ClientApp/build/index.html");

            // read the file
            var fileContents = await File.ReadAllTextAsync(indexPage.PhysicalPath);

            // find all script tags
            var scriptTags = Regex.Matches(fileContents, "<script.*?</script>", RegexOptions.Singleline);

            // get the script tags as strings
            var scriptTagsAsStrings = scriptTags.Select(m => m.Value).ToArray();

            var model = new DynamicScriptModel { Scripts = scriptTagsAsStrings };

            return View(model);
        }
    }

    public class DynamicScriptModel
    {
        public string[] Scripts { get; set; }
    }
}