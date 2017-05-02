using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AnlabMvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class VersionAttribute : ActionFilterAttribute
    {
        public int MajorVersion { get; set; }
        public string VersionKey { get; set; }

        public VersionAttribute()
        {
            MajorVersion = 1;
            VersionKey = "Version";
        }

        ///// <summary>
        ///// Grabs the date time stamp and places the version in Cache if it does not exist
        ///// and places the version in ViewData
        ///// </summary>
        ///// <param name="filterContext"></param>
        //private void LoadAssemblyVersion(ActionExecutingContext filterContext)
        //{
        //    var version = filterContext.HttpContext.Cache[VersionKey] as string;

        //    if (string.IsNullOrEmpty(version))
        //    {
        //        version = Assembly.GetExecutingAssembly().GetName().Version.ToString(); //Version from AppVeyor.

        //        //Insert version into the cache until tomorrow (Today + 1 day)
        //        filterContext.HttpContext.Cache.Insert(VersionKey, version, null, DateTime.Today.AddDays(1), Cache.NoSlidingExpiration);
        //    }

        //    filterContext.Controller.ViewData[VersionKey] = version;
        //}

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    LoadAssemblyVersion(filterContext);

        //    base.OnActionExecuting(filterContext);
        //}
    }
}
