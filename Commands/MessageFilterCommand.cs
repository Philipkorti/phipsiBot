using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.EventArgs;
using Services.Data;
using Services.DbServices;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enums.Enums;
using DbConnection.Entity;
using MySqlX.XDevAPI.Common;
using NLog;
using Microsoft.Extensions.Logging;

namespace Commands
{
    public class MessageFilterCommand : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public async static Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                if (args.Author.IsBot)
                {
                    return;
                }

                List<FilterWords> words = MessageFilterService.GetWordsList();
                string[] messageWords = args.Message.Content.Split(new char[] { ' ', '.', ',', '#', '+', '-', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (messageWords.Any(msgWord => words.Any(word => words.Any(w => w.Words.ToLower() == msgWord.ToLower()))))
                {
                    logger.Info($"Die Nachricht {args.Message} von {args.Author.Username} wurde gelöscht!");
                    await args.Message.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {args.Author.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),args.Author.Username);
            }
            
        }
        [Command("addFilter")]
        [GroupsService(Groups.Manager)]
        public async Task SetAddFilter(CommandContext ctx, string filter)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl addFilter benutzt!");
                if (!Authorized.Authentication.IsUserAuthorized(this,nameof(SetAddFilter),ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                    return;
                }

                List<FilterWords> filterwords = MessageFilterService.GetWordsList();
                FilterData data = WriteToJson.ReadJson("filter.json");
                string[] filters = filter.Split(',', ':', ';');
                List<string> duplicates = new List<string>();
                List<string> addedwords = new List<string>();
                foreach (string filterWord in filters)
                {
                    if(filterwords.Find(word => word.Words == filterWord) != null)
                    {
                        duplicates.Add(filterWord);
                        break;
                    }
                    MessageFilterService.AddWords(filterWord);
                    addedwords.Add(filterWord);
                }
                string msg = $"Die Liste von den Filter Wörter wurde um folgende erweitert: {string.Join(',', addedwords)}";
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData data = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("viewFilter")]
        [GroupsService(Groups.Manager)]
        public async Task ViewFilter(CommandContext ctx)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl viewFilter benutzt!");
                if (!Authorized.Authentication.IsUserAuthorized(this,nameof(ViewFilter), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                    return;
                }
                List<FilterWords> words = MessageFilterService.GetWordsList();
                string msg = $"Aktuelle Filter Liste: {string.Join(',', words)}";
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData data = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
