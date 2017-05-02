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

        private const string TempDataMessageKey = "Message";
        private const string TempDataErrorMessageKey = "ErrorMessage";

        public string Message
        {
            get => TempData[TempDataMessageKey] as string;
            set => TempData[TempDataMessageKey] = value;
        }

        public string ErrorMessage
        {
            get => TempData[TempDataErrorMessageKey] as string;
            set => TempData[TempDataErrorMessageKey] = value;
        }
    }
}
