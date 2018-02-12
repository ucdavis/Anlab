using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class EmailSettings
    {
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        public bool UseLiveEmails { get; set; } = false;
    }
}
