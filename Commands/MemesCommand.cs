using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public class MemesCommand : BaseCommandModule
    {
        string api = "https://api.imgflip.com/get_memes";
        [Command("meme")]
        public async Task GetRandomMemeAsync(CommandContext ctx)
        {
            using(HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(api);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    JArray meme = (JArray)json["data"]["memes"];

                    Random random = new Random();
                    int randomIndex = random.Next(meme.Count);
                    JObject randomMeme = (JObject)meme[randomIndex];

                    await ctx.Channel.SendMessageAsync(randomMeme["url"].ToString());
                }catch(Exception ex)
                {

                }
            }
        }
    }
}
