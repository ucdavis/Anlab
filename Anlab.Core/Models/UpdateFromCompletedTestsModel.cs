using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class UpdateFromCompletedTestsModel
    {
        public bool Confirm { get; set; }
        public IFormFile UploadFile { get; set; }
        public string LabComments { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
