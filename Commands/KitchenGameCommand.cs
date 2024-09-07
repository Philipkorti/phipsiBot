using DbConnection.Entity;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Imbiss.Classes;
using Imbiss.Services;
using Org.BouncyCastle.Bcpg.Sig;
using Services.Data;
using Services.DbServices;
using Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config;
using System.Web;

namespace Commands
{
    public class KitchenGameCommand : BaseCommandModule
    {
        Dictionary<string, KitchenGameData> games = new Dictionary<string, KitchenGameData>();
        [Command("kitchenCreate")]
        public async Task CreateGame(CommandContext ctx)
        {
            try
            {
                DiscordClient discordClient;
                DiscordChannel channel = await ctx.Guild.CreateChannelAsync($"KitchenGame - {ctx.User.Username}", ChannelType.Text,
                    overwrites: new List<DiscordOverwriteBuilder>
                    {
                    new DiscordOverwriteBuilder(ctx.Guild.EveryoneRole).Deny(Permissions.AccessChannels),
                    new DiscordOverwriteBuilder(ctx.Guild.CurrentMember)
                    .Allow(Permissions.AccessChannels)
                    .Allow(Permissions.SendMessages)
                    .Allow(Permissions.ReadMessageHistory),
                    new DiscordOverwriteBuilder(ctx.Member)
                    .Allow(Permissions.AccessChannels)
                    .Allow(Permissions.SendMessages)
                    .Allow(Permissions.ReadMessageHistory)
                    });
                await channel.CreateInviteAsync();
                games.Add(ctx.User.Username, new KitchenGameData() { ChannelId = channel.Id});
            }
            catch(Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("kitchenGameStop")]
        public async Task StopGame(CommandContext ctx)
        {
            try
            {
                KitchenGameData gameData = games[ctx.User.Username];
                if (gameData == null || ctx.Channel.Id != gameData.ChannelId)
                {
                    await ctx.Channel.SendMessageAsync("Du hast kein Game erstellt!");
                    return;
                }
                DiscordChannel channel = ctx.Guild.GetChannel(games[ctx.User.Username].ChannelId);
                await channel.DeleteAsync();
                games.Remove(ctx.User.Username);
            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("kitchenGameStart")]
        public async Task GameStart(CommandContext ctx)
        {
            try
            {
                KitchenGameData gameData = games[ctx.User.Username];
                if (gameData == null || ctx.Channel.Id != gameData.ChannelId)
                {
                    await ctx.Channel.SendMessageAsync("Du hast kein Game erstellt!");
                    return;
                }
                gameData.Game = new Game(ctx);

                DiscordChannel channel = ctx.Guild.GetChannel(games[ctx.User.Username].ChannelId);
                await channel.DeleteAsync();
                games.Remove(ctx.User.Username);
            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
            
        }

        [Command("kitchenGameStats")]
        public async Task GetKitchenGameStats(CommandContext ctx, string username = null)
        {
            try
            {
                string user;
                if (string.IsNullOrEmpty(username))
                {
                    user = ctx.User.Username;
                }
                else
                {
                    user = username;
                }

                KitchenGame kitchenGame = KitchenGameService.GetKichenGameByUsername(user);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.Title = "Kitchen Game Statistik";
                embedBuilder.AddField("Money", $"{kitchenGame.Money} Euro");
                embedBuilder.AddField("Rounds", $"{kitchenGame.Rounds} Runden");
                embedBuilder.AddField("Products sold", $"{kitchenGame.ProductsSold}");
                await ctx.Channel.SendMessageAsync(embedBuilder);
            }catch(Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
        }

        [Command("kitchenHelp")]
        public async Task GetCommands(CommandContext ctx)
        {
            try
            {
                var configReader = new ConfigReader();
                await configReader.ReadConfig();
                string prefix = configReader.Pefix;

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.Title = "Kitchen Game Commands";
                builder.AddField($"{prefix}kitchenCreate", "Erstellt ein extra Channel für das Game. Dieser Befehl muss als erstes ausgeführt werden.");
                builder.AddField($"{prefix}kitchenGameStart", "Dieser Befehl startet das Game. Dieser Befehl muss in dem neuen Channel benutzt werden.");
                builder.AddField($"{prefix}kitchenGameStats", "Dieser Befehl ruft die Statistik auf von einem bestimmten Spieler.");
                builder.AddField($"{prefix}kitchenGameStop", "Dieser Befehl beendet das Spiel.");
                builder.AddField($"{prefix}kitchenTop", "Gibt die Besten spieler zurück.");

                await ctx.Channel.SendMessageAsync(builder);
            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("kitchenTop")]
        public async Task GetKitchenTop(CommandContext ctx)
        {
            try
            {
                List<KitchenGame> kitchenGames = KitchenGameService.GetTopKitchenGameUser();
                User user;
                for (int i = 0; i < kitchenGames.Count; i++)
                {
                    user = UserServices.GetUserById(kitchenGames[i].UserId);
                    await ctx.Channel.SendMessageAsync($"{i+1}. {user.Username} mit {kitchenGames[i].Rounds / kitchenGames[i].CountGames} Runden im Durchschnitt");
                }
            }catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("kitchenBeschreibung")]
        public async Task GetBechreibung(CommandContext ctx)
        {
            try
            {
                DiscordEmbedBuilder discordEmbedBuilder = new DiscordEmbedBuilder();
                discordEmbedBuilder.Title = "Beschreibung";
                discordEmbedBuilder.Description = "In diesem rundenbasierten Spiel stehen dir in jeder Runde sechs verschiedene Aktionen zur Auswahl:\n" +
                    "1. Eier kaufen\n2. Tomaten kaufen\n3. Pilze kaufen\n4. Omletts verkaufen\n5. Schlechte Lebensmittel entsorgen\n6. Nichts tun\n" +
                    "jede Runde fordert strategische Entscheidungen von die, da die Omletts einen unterschiedlichen Verkaufswert haben. Doch Vorsicht: " +
                    "Solltest du keine Vorräte mehr haben, dein Geld aufgebraucht sein oder verdorbene Lebensmittel verkaufen, ist das Spiel vorbei!\n\n" +
                    "Die Preise der Produkte können sich im Verlauf des Spiels ändern, was deine Planung zusätzlich herausfordert. Nutze deine " +
                    "Ressourcen klug und versuche, das Spiel erfolgreich zu meistern!";
                await ctx.Channel.SendMessageAsync(discordEmbedBuilder);
            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace, DateTime.Now,ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
