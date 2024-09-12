using Config;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Enums.Enums;
using Newtonsoft.Json.Linq;
using NLog;
using Services.Data;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Commands
{
    public class SteamNewsCommands : BaseCommandModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string url = "http://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?";
        [Command("OverwatchNews")]
        public async Task GetDeadlookNews(CommandContext ctx)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl OverwatchNews benutzt!");
                ConfigReader configReader = new ConfigReader();
                configReader.ReadConfig();
                string url = this.url + $"appid=2357570&key={configReader.SteamAPI}&count=1";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(responseData);
                        var appDetails = json["appnews"]["newsitems"][0];
                        string name = appDetails["title"].ToString();
                        string description = appDetails["contents"].ToString();

                        await ctx.Channel.SendMessageAsync("**" + name + "**");
                        string html = SteamAPIService.ConvertHtmlToMarkdown(description);
                        foreach (var item in SteamAPIService.SplitMessage(html))
                        {
                            await ctx.Channel.SendMessageAsync(item);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex,$"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
           
        }
    }
}
