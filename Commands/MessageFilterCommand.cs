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
            FilterData filterData = WriteToJson.ReadJson("filter.json");

            if (filterData.FilterWords.Any(text => text.Contains(args.Message.Content)))
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
                FilterData data = WriteToJson.ReadJson("filter.json");
                string[] filters = filter.Split(',', ':', ';');
                data.FilterWords.AddRange(filters);
                WriteToJson.WriteJson(data, "filter.json");
                await ctx.Channel.SendMessageAsync($"Die Liste von den Filter Wörter wurde um folgende erweitert: {filter}");
            }
            catch (Exception ex)
            {
                ErrorTaskData data = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
        [Command("createFilter")]
        [GroupsService(Groups.Manager)]
        public async Task CreateFilter(CommandContext ctx, string title, string filter)
        {
            try
            {
                if(!Authorized.Authentication.IsUserAuthorized(this,nameof(CreateFilter), ctx.User.Username))
                {
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                    return;
                }
                string[] filters = filter.Split(',', ':', ';');
                FilterData data = new FilterData();
                data.Title = title;
                data.FilterWords = new List<string>();

                data.FilterWords.AddRange(filters);
                WriteToJson.WriteJson(data, "filter.json");
                await ctx.Channel.SendMessageAsync($"Der Filter wurde erfolgreich erstellt mit den folgenden Wörter: {filter}");

            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
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
                FilterData data = WriteToJson.ReadJson("filter.json");
                string filter = string.Join(",", data.FilterWords);
                await ctx.Channel.SendMessageAsync($"Aktuelle Filter Liste: {filter}");
            }catch (Exception ex)
            {
                ErrorTaskData data = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
