using Anlab.Core.Services;
using Shouldly;
using Xunit;

namespace Test.TestsServices
{
    public class HtmlEmailTextFormatterTests
    {
        [Fact]
        public void ToPlainText_RemovesHtmlChromeAndPreservesReadableContent()
        {
            var plainText = HtmlEmailTextFormatter.ToPlainText(
                @"<html>
                    <head>
                        <style>.button { color: red; }</style>
                        <title>Anlab</title>
                    </head>
                    <body>
                        <h1>Work Request 22F107</h1>
                        <p>Hello&nbsp;<strong>Client</strong>,</p>
                        <p>Your work order has been completed.</p>
                    </body>
                </html>");

            plainText.ShouldBe(
                @"Work Request 22F107

Hello Client,

Your work order has been completed.");
            plainText.ShouldNotContain("<");
            plainText.ShouldNotContain("button");
            plainText.ShouldNotContain("Anlab");
        }

        [Fact]
        public void ToPlainText_FormatsLinksWithTheirUrl()
        {
            var plainText = HtmlEmailTextFormatter.ToPlainText(
                @"<p>Results are ready.</p>
                  <p><a href=""https://localhost:5001/Results/Link/11111111-1111-1111-1111-111111111111"">Get Your Results</a></p>");

            plainText.ShouldBe(
                @"Results are ready.

Get Your Results: https://localhost:5001/Results/Link/11111111-1111-1111-1111-111111111111");
        }

        [Fact]
        public void ToPlainText_AddsSpacingBetweenTableCells()
        {
            var plainText = HtmlEmailTextFormatter.ToPlainText(
                @"<table>
                    <tr><th>Online Order Number</th><td>2920</td></tr>
                    <tr><th>Payment Method</th><td>IOC</td></tr>
                  </table>");

            plainText.ShouldBe(
                @"Online Order Number 2920

Payment Method IOC");
        }
    }
}
