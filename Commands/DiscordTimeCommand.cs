using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using Services.DbServices;
using Services.Data;
using Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnection.Entity;

namespace Commands
{
    public class DiscordTimeCommand : BaseCommandModule
    {
        public static Task OnVoiceStateUpdate(DiscordClient sender, VoiceStateUpdateEventArgs args)
        {
            try
            {
                if (args.Before?.Channel == null && args.After?.Channel != null)
                {
                    TimeHelperService.SetJoinTime(args.User.Username);
                }

                if (args.Before?.Channel != null && args.After?.Channel == null)
                {
                    DateTime dateTime = DateTime.Now;
                    DateTime joinTime = TimeHelperService.GetJoinTime(args.User.Username);
                    TimeSpan difference = dateTime - joinTime;
                    UserServices.SetUserTime(args.User.Username, Convert.ToInt64(difference.TotalSeconds));
                    TimeHelperService.ClearJoinTime(args.User.Username);
                }
            }catch (Exception ex)
            {
                ErrorTaskData data = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now, ErrorTaskStatus.NEW.ToString(), sender.CurrentUser.Username);
            }
            
            return Task.CompletedTask;
        }

        [Command("time")]
        public async Task GetTime(CommandContext ctx, string username = null)
        {
            Int64 time;
            try
            {
                string name = username == null ? ctx.User.Username : username;
                if (ctx.Member?.VoiceState?.Channel != null)
                {
                    DateTime dateTime = DateTime.Now;
                    DateTime joinTime = TimeHelperService.GetJoinTime(name);
                    if (joinTime != null)
                    {
                        TimeSpan difference = dateTime - joinTime;
                        UserServices.SetUserTime(name, Convert.ToInt64(difference.TotalSeconds));
                        TimeHelperService.ClearJoinTime(name);
                        TimeHelperService.SetJoinTime(name);
                    }

                }
                time = UserServices.GetUserTimeByUsername(name);
                long hour = time / 3600;
                long reminingSeconds = time % 3600;
                long minutes = reminingSeconds / 60;
                await ctx.Channel.SendMessageAsync($"Die Discord Zeit von {name} beträgt {hour} Stunden und {minutes} Minuten.");
            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
            
        }

        [Command("timetop")]
        public async Task GetTimeTop(CommandContext ctx)
        {
            try
            {
                List<User> users = TimeHelperService.GetTopUsers();

                for (int i = 0; i < users.Count; i++)
                {
                    long hour = users[i].TimeInSecond / 3600;
                    long reminingSeconds = users[i].TimeInSecond % 3600;
                    long minutes = reminingSeconds / 60;
                    await ctx.Channel.SendMessageAsync($"{i+1}: {users[i].Username} mit {hour} Stunden und {minutes} Minuten");
                }

            }catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
        }
    }
}
