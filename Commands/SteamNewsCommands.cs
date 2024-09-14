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
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Commands
{
    public class SteamNewsCommands : BaseCommandModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        [Command("OverwatchNews")]
        public async Task GetOverwatchNews(CommandContext ctx, string appid)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl OverwatchNews benutzt!");
                string name = SteamAPIService.GetOverwatchNews(out IEnumerable<string> message, appid);
                await ctx.Channel.SendMessageAsync("**" + name + "**");
                foreach (var item in message)
                {
                    await ctx.Channel.SendMessageAsync(item);
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
