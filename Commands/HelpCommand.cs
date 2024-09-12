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
                var readconfig = new ConfigReader();
                await readconfig.ReadConfig();
                string prefix = readconfig.Pefix;
                User user = UserServices.GetUerByUsername(ctx.User.Username);
                Groups groups = user.Groups;
                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.Title = "Hilfe";
                if (groups <= Groups.User)
                {
                    builder.AddField($"{prefix}time", "Gibt die aktuelle Discord Zeit wieder.");
                    builder.AddField($"{prefix}time Username", "Gibt die Zeit von einem Bestimmten Benutzer wieder!");
                    builder.AddField($"{prefix}timetop", "Gibt die 5 Spieler an mit der meisten Zeit.");
                    builder.AddField($"{prefix}meme", "Gibt ein zufälliges Meme wieder.");
                    builder.AddField($"{prefix}kitchenHelp", "Gibt die Hilfe zurück für kitchenGame.");

                    if (groups <= Groups.Manager)
                    {
                        builder.AddField($"{prefix}createFilter title filterWörter", "Erstellt eine neue Filterliste. Es können mehrere filter Wörter hinzugefügt werden mit dem Trennzeichen ','.");
                        builder.AddField($"{prefix}addFilter filterWörter", "fügt zu der Filterliste Wörter hinzu. Es können mehrere Wörter angegeben werden mit dem Trennzeichen ','.");
                        builder.AddField($"{prefix}viewFilter", "Zeigt die Aktuelle Filterliste an.");

                        if (groups <= Groups.Admin)
                        {
                            builder.AddField($"{prefix}setGroup Username Group", "Setzt die Gruppe von einem Spieler. Die Folgende Gruppen gibt es: User, Manager, Admin.");
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
