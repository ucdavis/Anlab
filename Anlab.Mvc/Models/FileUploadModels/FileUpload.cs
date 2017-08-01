using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.FileUploadModels
{
    public class FileUpload
    {
        public string Identifier { get; set; }
        public string ContentType { get; set; }
        public string CacheControl { get; set; }
        public Stream Data { get; set; }
    }
}
