using DbConnection.Entity;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Enums.Enums;
using Services.DbServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public class HelpCommand : BaseCommandModule
    {
        [Command("help")]
        public async Task Help(CommandContext ctx)
        {
            User user = UserServices.GetUerByUsername(ctx.User.Username);
            Groups groups = user.Groups;
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.Title = "Hilfe";
           if(groups <= Groups.User)
            {
                builder.AddField("!time", "Gibt die aktuelle Discord Zeit wieder.");
                builder.AddField("!time Username", "Gibt die Zeit von einem Bestimmten Benutzer wieder!");
                builder.AddField("!timetop", "Gibt die 5 Spieler an mit der meisten Zeit.");
                builder.AddField("!meme", "Gibt ein zufälliges Meme wieder.");

                if(groups <= Groups.Manager)
                {
                    builder.AddField("!createFilter title filterWörter", "Erstellt eine neue Filterliste. Es können mehrere filter Wörter hinzugefügt werden mit dem Trennzeichen ','.");
                    builder.AddField("!addFilter filterWörter", "fügt zu der Filterliste Wörter hinzu. Es können mehrere Wörter angegeben werden mit dem Trennzeichen ','.");
                    builder.AddField("!viewFilter", "Zeigt die Aktuelle Filterliste an.");

                    if(groups <= Groups.Admin)
                    {
                        builder.AddField("!setGroup Username Group", "Setzt die Gruppe von einem Spieler. Die Folgende Gruppen gibt es: User, Manager, Admin.");
                    }
                }
            }
            await ctx.Channel.SendMessageAsync(builder);
        }
    }
}
