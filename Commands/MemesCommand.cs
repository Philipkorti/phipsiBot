using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Services.Data;
using Enums.Enums;

namespace Commands
{
    public class MemesCommand : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        string api = "https://api.imgflip.com/get_memes";
        [Command("meme")]
        public async Task GetRandomMemeAsync(CommandContext ctx)
        {
            using(HttpClient client = new HttpClient())
            {
                try
                {
                    logger.Info($"Der User {ctx.User.Username} hat den Befehl meme benutzt!");
                    HttpResponseMessage response = await client.GetAsync(api);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    JArray meme = (JArray)json["data"]["memes"];

                    Random random = new Random();
                    int randomIndex = random.Next(meme.Count);
                    JObject randomMeme = (JObject)meme[randomIndex];

                    var embed = new DiscordEmbedBuilder()
                        .WithTitle(randomMeme["name"].ToString())
                        .WithImageUrl(randomMeme["url"].ToString())
                        .WithColor(DiscordColor.Blue)
                        .Build();

                    await ctx.Channel.SendMessageAsync(embed);

                }
                catch(Exception ex)
                {
                    logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                    ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
                }
            }
        }
    }
}
