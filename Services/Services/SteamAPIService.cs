using Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Services.ConstVariablen;
using System.Reflection;
using System.Data.SqlTypes;
using Services.DbServices;
using DbConnection.Entity;

namespace Services.Services
{
    public class SteamAPIService
    {
        private static IEnumerable<string> SplitMessage(string message)
        {
            int chunkSize = 2000;
            for (int i = 0; i < message.Length; i += chunkSize)
            {
                yield return message.Substring(i, Math.Min(chunkSize, message.Length - i));
            }
        }

        private static string ConvertHtmlToMarkdown(string html)
        {
            html = html.Replace("[img]", "");
            html = html.Replace("[/img]", "");
            html = html.Replace("{STEAM_CLAN_IMAGE}", "https://clan.akamai.steamstatic.com/images");
            html = Regex.Replace(html, @"\[h2](.*?)\[/h2]", "**$1**\n");
            html = Regex.Replace(html, @"<h3>(.*?)</h3>", "**$1**\n");
            return html;
        }

        public static string GetOverwatchNews(out IEnumerable<string> message, string appId, string name = null )
        {
            message = null;
            ConfigReader configReader = new ConfigReader();
            configReader.ReadConfig();
            string url = ConstVariablen.ConstVariablen.SteamAPI + $"appid={appId}&key={configReader.SteamAPI}&count=1";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(responseData);
                    var appDetails = json["appnews"]["newsitems"][0];
                    string title = null;
                    string description = appDetails["contents"].ToString();
                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(appDetails["date"].ToString())).UtcDateTime;
                    if(name != null)
                    {
                        ReadNews readNews = ReadNewsService.GetGameNewsDateByName(name);

                        if (readNews != null)
                        {
                            if (dateTime > readNews.LastUpdate || name == null)
                            {
                                ReadNewsService.SetGameNewsDateByName(dateTime, name);
                                title = appDetails["title"].ToString();
                                string html = SteamAPIService.ConvertHtmlToMarkdown(description);
                                message = SteamAPIService.SplitMessage(html);
                            }
                        }
                    }
                    else
                    {
                        title = appDetails["title"].ToString();
                        string html = SteamAPIService.ConvertHtmlToMarkdown(description);
                        message = SteamAPIService.SplitMessage(html);
                    }
                    
                    
                    
                    return title;
                }
            }
            return "";
        }
    }
}
