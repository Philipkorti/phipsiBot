using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SteamAPIService
    {
        public static IEnumerable<string> SplitMessage(string message)
        {
            int chunkSize = 2000;
            for (int i = 0; i < message.Length; i += chunkSize)
            {
                yield return message.Substring(i, Math.Min(chunkSize, message.Length - i));
            }
        }

        public static string ConvertHtmlToMarkdown(string html)
        {
            html = html.Replace("[img]", "");
            html = html.Replace("[/img]", "");
            html = html.Replace("{STEAM_CLAN_IMAGE}", "https://clan.akamai.steamstatic.com/images");
            html = Regex.Replace(html, @"\[h2](.*?)\[/h2]", "**$1**\n");
            html = Regex.Replace(html, @"<h3>(.*?)</h3>", "**$1**\n");
            return html;
        } 
    }
}
