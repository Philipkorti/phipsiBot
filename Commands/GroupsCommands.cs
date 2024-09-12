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
using NLog;
using Services.Data;
using Services.Enums;

namespace Commands
{
    public class GroupsCommands : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        [Command("setGroup")]
        [GroupsService(Groups.Admin)]
        public async Task SetGroup(CommandContext ctx, string username, string group)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl setGroup benutzt!");
                if (!Authentication.IsUserAuthorized(this, nameof(SetGroup), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
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
                string msg = $"Die Gruppe wurde von {username} wurde auf {groups.ToString()} gesetzt!";
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
                UserServices.SetGroupByUsername(username, groups);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
            
        }
    }
}
