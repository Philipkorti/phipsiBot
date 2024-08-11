using DbConnection.Context;
using DbConnection.Entity;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Services;
using Enums.Enums;
using Commands.Authorized;
using Services.DbServices;

namespace Commands
{
    public class GroupsCommands : BaseCommandModule
    {
        [Command("setGroup")]
        [GroupsService(Groups.Admin)]
        public async Task SetGroup(CommandContext ctx, string username, string group)
        {
            if (!Authentication.IsUserAuthorized(this, nameof(SetGroup), ctx.User.Username))
            {
                await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                return;
            }
            Groups groups = Groups.User;
            switch (group)
            {
                case "User":
                    {
                        groups = Groups.User;
                        break;
                    }
                case "Manager":
                    {
                        groups = Groups.Manager;
                        break;
                    }
                case "Admin":
                    {
                        groups = Groups.Admin;
                        break;
                    }
                default:
                    {
                        await ctx.Channel.SendMessageAsync("Die Gruppe die sie angegeben haben gibt es nicht!");
                        return;
                    }
            }
            await ctx.Channel.SendMessageAsync($"Die Gruppe wurde von {username} wurde auf {groups.ToString()}");
            UserServices.SetGroupByUsername(username, groups);
        }
    }
}
