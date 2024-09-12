using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Services.Data;
using Services.DbServices;
using Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;
using Services.Services;
using Enums.Enums;
using Commands.Authorized;
using NLog;

namespace Commands
{
    public class AddUser : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        [Command("addUser")]
        [GroupsService(Groups.Manager)]
        public async Task AddUsername(CommandContext context)
        {
            try
            {
                logger.Info($"Der User {context.User.Username} hat den Befehl addUser benutzt!");
                if (!Authentication.IsUserAuthorized(this, nameof(AddUser), context.User.Username))
                {
                    logger.Warn($"Der User {context.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await context.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                UserServices.AddUser(context.User.Username);
                await context.Channel.SendMessageAsync($"Der Benutzer {context.User.Username} wurde in der DatenBank erstellt!");
            }catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {context.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now,ErrorTaskStatus.NEW.ToString(), context.User.Username);
            }
            
        }
    }
}
