using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.EventArgs;
using Services.Data;
using Services.DbServices;
using Services.Enums;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enums.Enums;
using DbConnection.Entity;
using MySqlX.XDevAPI.Common;

namespace Commands
{
    public class MessageFilterCommand : BaseCommandModule
    {
        public async static Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs args)
        {
            if(args.Author.IsBot)
            {
                return;
            }

            List<FilterWords> words = MessageFilterService.GetWordsList();
            string[] messageWords = args.Message.Content.Split(' ',StringSplitOptions.RemoveEmptyEntries);
            if(messageWords.Any(msgWord => words.Any(word => words.Any(w => w.Words.ToLower() == msgWord.ToLower()))))
            {
                await args.Message.DeleteAsync();
            }
        }
        [Command("addFilter")]
        [GroupsService(Groups.Manager)]
        public async Task SetAddFilter(CommandContext ctx, string filter)
        {
            try
            {
                if (!Authorized.Authentication.IsUserAuthorized(this,nameof(SetAddFilter),ctx.User.Username))
                {
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
                await ctx.Channel.SendMessageAsync($"Die Liste von den Filter Wörter wurde um folgende erweitert: {string.Join(',',addedwords)}");
            }
            catch (Exception ex)
            {
                ErrorTaskData data = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("viewFilter")]
        [GroupsService(Groups.Manager)]
        public async Task ViewFilter(CommandContext ctx)
        {
            try
            {
                if(!Authorized.Authentication.IsUserAuthorized(this,nameof(ViewFilter), ctx.User.Username))
                {
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                    return;
                }
                List<FilterWords> words = MessageFilterService.GetWordsList();
                await ctx.Channel.SendMessageAsync($"Aktuelle Filter Liste: {string.Join(',', words)}");
            }catch (Exception ex)
            {
                ErrorTaskData data = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
