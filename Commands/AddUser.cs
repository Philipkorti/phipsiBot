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

namespace Commands
{
    public class AddUser : BaseCommandModule
    {
        [Command("addUser")]
        public async Task AddUsername(CommandContext context)
        {
            try
            {
                UserServices.AddUser(context.User.Username);
                await context.Channel.SendMessageAsync($"Der Benutzer {context.User.Username} wurde in der DatenBank erstellt!");
            }catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now,ErrorTaskStatus.NEW.ToString(), context.User.Username);
            }
            
        }
    }
}
