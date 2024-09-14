using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using Services.DbServices;
using Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnection.Entity;
using Services.Services;
using Enums.Enums;
using Commands.Authorized;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NLog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Commands
{
    public class DiscordTimeCommand : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public static Task OnVoiceStateUpdate(DiscordClient sender, VoiceStateUpdateEventArgs args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                if (args.Before?.Channel == null && args.After?.Channel != null)
                {
                    logger.Info($"Der User {args.User.Username} ist dem Channel {args.After.Channel} beigetreten!");
                    TimeHelperService.SetJoinTime(args.User.Username);
                }

                if (args.Before?.Channel != null && args.After?.Channel == null)
                {
                    logger.Info($"Der User {args.User.Username} hat den Channel {args.Before.Channel} verlassen!");
                    DateTime dateTime = DateTime.Now;
                    DateTime joinTime = TimeHelperService.GetJoinTime(args.User.Username);
                    TimeSpan difference = dateTime - joinTime;
                    UserServices.SetUserTime(args.User.Username, Convert.ToInt64(difference.TotalSeconds));
                    TimeHelperService.ClearJoinTime(args.User.Username);
                }
            }catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {args.User.Username}");
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
                logger.Info($"Der User {ctx.User.Username} hat den Befehl time benutzt!");
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
                reminingSeconds = reminingSeconds % 60;
                string msg = $"Die Discord Zeit von {name} beträgt {hour} Stunden und {minutes} Minuten und {reminingSeconds} Sekunden.";
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
            
        }

        [Command("timetop")]
        public async Task GetTimeTop(CommandContext ctx)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl timetop benutzt!");
                List<User> users = TimeHelperService.GetTopUsers();

                for (int i = 0; i < users.Count; i++)
                {
                    long hour = users[i].TimeInSecond / 3600;
                    long reminingSeconds = users[i].TimeInSecond % 3600;
                    long minutes = reminingSeconds / 60;
                    reminingSeconds = reminingSeconds % 60;
                    string msg = $"{i + 1}: {users[i].Username} mit {hour} Stunden und {minutes} Minuten und {reminingSeconds} Sekunden";
                    logger.Info(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                }

            }catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
        }

        [GroupsService(Groups.Admin)]
        [Command("addTime")]
        public async Task AddTime(CommandContext ctx, string username,long hour, long minutes, long seconds)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl addTime benutzt!");
                if (!Authentication.IsUserAuthorized(this, nameof(AddTime), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Command nicht ausführen!");
                    return;
                }
                long setSeconds = hour*3600 + minutes*60 + seconds;
                UserServices.SetUserTime(username, setSeconds);
                string msg = $"Es wurden an den Benutzer {username} {hour} Stunden {minutes} Minuten {seconds} Sekunden das entspricht {setSeconds} Sekunden hinzugefügt.";
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("TimeInSeconds")]
        public async Task GetTimeInSecond(CommandContext ctx, string username = null)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl TimeInSecond benutzt!");
                username = string.IsNullOrEmpty(username) ? ctx.User.Username : username;
                if (ctx.Member?.VoiceState?.Channel != null)
                {
                    DateTime dateTime = DateTime.Now;
                    DateTime joinTime = TimeHelperService.GetJoinTime(username);
                    if (joinTime != null)
                    {
                        TimeSpan difference = dateTime - joinTime;
                        UserServices.SetUserTime(username, Convert.ToInt64(difference.TotalSeconds));
                        TimeHelperService.ClearJoinTime(username);
                        TimeHelperService.SetJoinTime(username);
                    }

                }

                User user = UserServices.GetUerByUsername(username);
                string msg = $"Die Discord Zeit von {username} beträgt {user.TimeInSecond} Sekunden.";
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
