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
using NLog;
using DbConnection.Entity;
using Services.DbServices;
using DSharpPlus.Entities;
using Services.Data;
using Services.Enums;

namespace Commands
{
    public class LogCommand : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        [Command("Log")]
        [GroupsService(Groups.Admin)]
        public async Task GetLog(CommandContext ctx, string date = null, string loglevel = null)
        {
            try
            {
                if (!Authentication.IsUserAuthorized(this, nameof(GetLog), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }

                List<Logging> logs = LogService.GetLogs(date, loglevel);

                string fileName = $"{DateTime.Now.ToString("dd-MM-yyyy")}-log.txt";
                File.Create(fileName).Close();
                string line;
                using (StreamWriter stream = new StreamWriter(fileName))
                {
                    foreach (Logging log in logs)
                    {
                        line = $"{log.date}|{log.LogLevel}| {log.Logger} {log.Message} | {log.Exception}";
                        stream.WriteLine(line);
                    }
                }
                DiscordMessageBuilder builder = new DiscordMessageBuilder();

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    builder.AddFile(fileName, fs);
                    await ctx.Channel.SendMessageAsync(builder);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
        }
    }
}
