using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Enums.Enums;
using Language;
using NLog;
using Services.Data;
using Services.DbServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public class LanguageCommand : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        [Command("setlanguage")]
        public async Task SetLanguage(CommandContext ctx, string language)
        {
            try
            {
                BotLocalization botLocalization = new BotLocalization(language);
                UserServices.SetLanguageCodeByUsername(ctx.User.Username, language);

                await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("editLanguage",language));
            }catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
