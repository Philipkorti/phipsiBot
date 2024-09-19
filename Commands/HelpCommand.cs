using DbConnection.Entity;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Enums.Enums;
using Services.Data;
using Services.DbServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config;
using NLog;
using Language;

namespace Commands
{
    public class HelpCommand : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        [Command("help")]
        public async Task Help(CommandContext ctx)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl help benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                var readconfig = new ConfigReader();
                await readconfig.ReadConfig();
                string prefix = readconfig.Pefix;
                User user = UserServices.GetUerByUsername(ctx.User.Username);
                Groups groups = user.Groups;
                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.Title = "Hilfe";
                if (groups <= Groups.User)
                {
                    builder.AddField($"{prefix}time", botLocalization.GetLocalizedString("commandTime"));
                    builder.AddField($"{prefix}time Username", botLocalization.GetLocalizedString("commandTimeUser"));
                    builder.AddField($"{prefix}timetop", botLocalization.GetLocalizedString("commandTimeTop"));
                    builder.AddField($"{prefix}meme", botLocalization.GetLocalizedString("commandMeme"));
                    builder.AddField($"{prefix}kitchenHelp", botLocalization.GetLocalizedString("commandKitchenHelp"));
                    builder.AddField($"{prefix}OverwatchNews", botLocalization.GetLocalizedString("commandOverwatchNews"));
                    builder.AddField($"{prefix}setlanguage", botLocalization.GetLocalizedString("commandSetLanguage"));

                    if (groups <= Groups.Manager)
                    {
                        builder.AddField($"{prefix}createFilter title filterWörter", botLocalization.GetLocalizedString("commandcreateFilter"));
                        builder.AddField($"{prefix}addFilter filterWörter", botLocalization.GetLocalizedString("CommandAddFilter"));
                        builder.AddField($"{prefix}viewFilter", botLocalization.GetLocalizedString("commandViewFilter"));
                        builder.AddField($"{prefix}services", botLocalization.GetLocalizedString("commandServices"));
                        builder.AddField($"{prefix}readnews", botLocalization.GetLocalizedString("commandReadNews"));
                        builder.AddField($"{prefix}setchannelId serviceName", botLocalization.GetLocalizedString("commandSetChannelId"));
                        builder.AddField($"{prefix}addgame serviceName gameVerbindung newsType", botLocalization.GetLocalizedString("commandAddGame"));
                        builder.AddField($"{prefix}removeReadNews readNewsName", botLocalization.GetLocalizedString("commandRemoveReadNews"));

                        if (groups <= Groups.Admin)
                        {
                            builder.AddField($"{prefix}setGroup Username Group", botLocalization.GetLocalizedString("commandSetGroup"));
                            builder.AddField($"{prefix}create interval intervalTyp", botLocalization.GetLocalizedString("commandCreate"));
                            builder.AddField($"{prefix}start serviceName", botLocalization.GetLocalizedString("commandStart"));
                            builder.AddField($"{prefix}stop serviceName", botLocalization.GetLocalizedString("commandStop"));
                            builder.AddField($"{prefix}removeService serviceName", botLocalization.GetLocalizedString("commandRemoveService"));
                        }
                    }
                }
                await ctx.Channel.SendMessageAsync(builder);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
           
        }
    }
}
