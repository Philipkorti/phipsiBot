using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Services;
using Enums.Enums;
using Commands.Authorized;
using System.Reflection;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using DSharpPlus;

namespace Commands
{
    public class TestCommand : BaseCommandModule
    {
        [Command("test")]
        [GroupsService(Groups.Admin)]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            if(Authentication.IsUserAuthorized(this,nameof(MyFirstCommand),ctx.User.Username))
            {
                var builder = new DiscordMessageBuilder()
            .WithContent("Click the button below:")
            .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Primary, "button1", "Click Me!"),
                new DiscordButtonComponent(ButtonStyle.Secondary, "button2", "Or Me!")
            );

                var message = await ctx.Channel.SendMessageAsync(builder);
                await ctx.Channel.SendMessageAsync($"Hello {ctx.User.Username}");
                var interactivity = ctx.Client.GetInteractivity();
                var result = await interactivity.WaitForButtonAsync(message, TimeSpan.FromMinutes(1));
            }
            
        }
    }
}
