using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Pages
{
    public class MarkdownPage
    {
        public string Html { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public MarkdownPageMeta Meta { get; set; }
    }

    public class MarkdownPageMeta
    {
        public string Title { get; set; }
    }
}
