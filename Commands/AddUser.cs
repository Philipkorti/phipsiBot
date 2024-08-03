using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Services.DbServices;
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
                throw new NotImplementedException();
                UserServices.AddUser(context.User.Username);
                await context.Channel.SendMessageAsync($"Der Benutzer {context.User.Username} wurde in der DatenBank erstellt!");
            }catch (Exception ex)
            {
                ErrorTaskService.AddErrorTask(ex.Message, ex.StackTrace, context.User.Username);
            }
            
        }
    }
}
