using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Anlab.Core.Services
{
    public static class HtmlEmailTextFormatter
    {
        private const RegexOptions HtmlRegexOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant;

        public static string ToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var text = html;
            text = Regex.Replace(text, @"<!--.*?-->", string.Empty, HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*(head|style|script)[^>]*>.*?<\s*/\s*\1\s*>", string.Empty, HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*br\s*/?\s*>", "\n", HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*/\s*(p|div|section|article|header|footer|h[1-6]|li|tr|table|mj-section|mj-column)\s*>", "\n", HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*(p|div|section|article|header|footer|h[1-6]|li|tr|table|mj-section|mj-column)[^>]*>", "\n", HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*/\s*(td|th)\s*>", "  ", HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*(td|th)[^>]*>", string.Empty, HtmlRegexOptions);
            text = Regex.Replace(text, @"<\s*a\b[^>]*href\s*=\s*(?:""(?<href>[^""]*)""|'(?<href>[^']*)'|(?<href>[^\s>]+))[^>]*>(?<text>.*?)<\s*/\s*a\s*>", FormatLink, HtmlRegexOptions);
            text = Regex.Replace(text, @"<[^>]+>", string.Empty, HtmlRegexOptions);
            text = WebUtility.HtmlDecode(text);
            text = text.Replace('\u00A0', ' ');
            text = text.Replace("\r\n", "\n").Replace('\r', '\n');
            text = Regex.Replace(text, @"[ \t\f\v]+", " ");
            text = Regex.Replace(text, @" *\n *", "\n");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }

        private static string FormatLink(Match match)
        {
            var linkText = ToPlainText(match.Groups["text"].Value);
            var href = WebUtility.HtmlDecode(match.Groups["href"].Value ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(linkText))
            {
                return href;
            }

            if (string.IsNullOrWhiteSpace(href) || string.Equals(linkText, href, StringComparison.OrdinalIgnoreCase))
            {
                return linkText;
            }

            return $"{linkText}: {href}";
        }
    }
}
