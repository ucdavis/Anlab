using System;
using System.Reflection;
using AnlabMvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace AnlabMvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class VersionAttribute : ActionFilterAttribute
    {
        public int MajorVersion { get; set; }
        public string VersionKey { get; set; }
        private IMemoryCache _cache;

        public VersionAttribute(IMemoryCache cache)
        {
            _cache = cache;
            MajorVersion = 1;
            VersionKey = "Version";
        }

        /// <summary>
        /// Grabs the date time stamp and places the version in Cache if it does not exist
        /// and places the version in ViewData
        /// </summary>
        /// <param name="filterContext"></param>
        private void LoadAssemblyVersion(ActionExecutingContext filterContext)
        {
            string version = null;
            if (!_cache.TryGetValue(VersionKey, out version))
            {
                version = typeof(HomeController).GetTypeInfo()
                    .Assembly.GetName()
                    .Version
                    .ToString();
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Today.AddDays(1));

                _cache.Set(VersionKey, version, cacheOptions);

            }

            filterContext.Controller.TempData[VersionKey] = version;
        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    LoadAssemblyVersion(filterContext);

        //    base.OnActionExecuting(filterContext);
        //}
    }
}
