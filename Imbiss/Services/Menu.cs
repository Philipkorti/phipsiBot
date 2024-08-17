using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using System.Numerics;

namespace Imbiss.Services
{
    public class Menu
    {
        public static int ShowMenu(CommandContext ctx)
        {
            //DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            //embedBuilder.AddField(" ", "1. Play");
            //embedBuilder.AddField(" ", "2. Exit");
            ctx.Channel.SendMessageAsync("1. Play\n2. Exit");
            var interactivity = ctx.Client.GetInteractivity();
            bool isnum;
            int result;
            do
            {
                var message = interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id);

                isnum = int.TryParse(message.Result.Result.Content, out result);

            } while (!isnum);

            return result;

        }
    }
}
