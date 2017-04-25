using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AnlabMvc.Controllers
{
    public class ApplicationController : Controller
    {
        public string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
