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

namespace Commands
{
    public class AddUser : BaseCommandModule
    {
        [Command("addUser")]
        [GroupsService(Groups.Manager)]
        public async Task AddUsername(CommandContext context)
        {
            try
            {
                if(!Authentication.IsUserAuthorized(this, nameof(AddUser), context.User.Username))
                {
                    await context.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                UserServices.AddUser(context.User.Username);
                await context.Channel.SendMessageAsync($"Der Benutzer {context.User.Username} wurde in der DatenBank erstellt!");
            }catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now,ErrorTaskStatus.NEW.ToString(), context.User.Username);
            }
            
        }
    }
}
