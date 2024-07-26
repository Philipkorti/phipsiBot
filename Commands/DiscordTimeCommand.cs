using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using Services.DbServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            }
            
            return Task.CompletedTask;
        }

        [Command("time")]
        public async Task GetTime(CommandContext ctx)
        {
            Int64 time;
            if (ctx.Member?.VoiceState?.Channel != null)
            {
                DateTime dateTime = DateTime.Now;
                DateTime joinTime = TimeHelperService.GetJoinTime(ctx.User.Username);
                TimeSpan difference = dateTime - joinTime;
                UserServices.SetUserTime(ctx.User.Username, Convert.ToInt64(difference.TotalSeconds));
                TimeHelperService.ClearJoinTime(ctx.User.Username);
                TimeHelperService.SetJoinTime(ctx.User.Username);
            }
            time = UserServices.GetUserTimeByUsername(ctx.User.Username);
            long hour = time / 3600;
            long reminingSeconds = time % 3600;
            long minutes = reminingSeconds / 60;
            await ctx.Channel.SendMessageAsync($"Die Discord Zeit von {ctx.User.Username} beträgt {hour} Stunden und {minutes} Minuten.");
        }
    }
}
