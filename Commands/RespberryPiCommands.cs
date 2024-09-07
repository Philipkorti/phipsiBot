using Commands.Authorized;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Services;
using Enums.Enums;

namespace Commands
{
    public class RespberryPiCommands : BaseCommandModule
    {
        [GroupsService(Groups.Admin)]
        [Command("pitemp")]
        public async Task GetPiTemp(CommandContext ctx)
        {
            if (!Authentication.IsUserAuthorized(this, nameof(GetPiTemp), ctx.User.Username))
            {
                await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                return;
            }
            string temp = PiTemp.GetTemp();

            await ctx.Channel.SendMessageAsync($"Die aktuelle CPU-Temperatur beträgt: {temp}");
        }
    }
}
