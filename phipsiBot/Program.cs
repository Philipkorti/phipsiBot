// See https://aka.ms/new-console-template for more information
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Config;
using DSharpPlus.EventArgs;
using Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DbConnection.Context;
using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.Events;
using DSharpPlus.Entities;
using Services.DbServices;
using DSharpPlus.Interactivity.Extensions;
using NLog;

public class Program
{
    private static DiscordClient client { get; set; }
    private static CommandsNextExtension commands { get; set; }

    private static Logger logger { get; set; }

    static async Task Main(string[] args)
    {
        var configReader = new ConfigReader();
        await configReader.ReadConfig();
        NLog.GlobalDiagnosticsContext.Set("connectionString", configReader.dbConnection);
        logger = LogManager.GetCurrentClassLogger();
        logger.Info("Discord Bot wird gestartet");
        
        logger.Info("Die config wurde geladen!");
        ErrorTaskData.ObjectCreated += ErrorTaskData_ObjectCreated;

        var discordConfig = new DiscordConfiguration()
        {
            Intents = DiscordIntents.All,
            Token = configReader.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };
        logger.Info("Die Discord Config wurde festgelegt!");
        client = new DiscordClient(discordConfig);
        client.Ready += client_Ready;
        client.VoiceStateUpdated += DiscordTimeCommand.OnVoiceStateUpdate;
        client.MessageCreated += MessageFilterCommand.OnMessageCreated;
        client.UseInteractivity();
        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new string[] { configReader.Pefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
        };
        commands = client.UseCommandsNext(commandsConfig);
        commands.RegisterCommands<TestCommand>();
        logger.Info("Die TestCommand Klasse wurde registriert!");
        commands.RegisterCommands<AddUser>();
        logger.Info("Die AddUserCommand Klasse wurde registriert!");
        commands.RegisterCommands<DiscordTimeCommand>();
        logger.Info("Die DiscordTimeCommand Klasse wurde registriert!");
        commands.RegisterCommands<MemesCommand>();
        logger.Info("Die MemesCommand Klasse wurde registriert!");
        commands.RegisterCommands<GroupsCommands>();
        logger.Info("Die GroupsCommand Klasse wurde registriert!");
        commands.RegisterCommands<MessageFilterCommand>();
        logger.Info("Die MessageFilterCommand wurde registriert");
        commands.RegisterCommands<HelpCommand>();
        logger.Info("Die HelpCommand Klasse wurde registriert!");
        commands.RegisterCommands<KitchenGameCommand>();
        logger.Info("Die KitchenGameCommand Klasse wurde registriert!");
        commands.RegisterCommands<RespberryPiCommands>();
        commands.RegisterCommands<SteamNewsCommands>();
        logger.Info("Die SteamNewsCommands Klasse wurde registriert!");
        commands.RegisterCommands<LogCommand>();
        logger.Info("Die LogCommand Klasse wurde registriert!");
        logger.Info("Die Verbindung zu Discord wird aufgebaut!");
        await client.ConnectAsync();
        logger.Info("Der Discord Bot ist nun bereit!");
        await Task.Delay(-1);
    }

    private static void ErrorTaskData_ObjectCreated(object sender, ErrorTaskArgs e)
    {
        foreach(var guild in client.Guilds)
        {
            DiscordMember client =  guild.Value.GetMemberAsync(327492578616410123).Result;
            if ((client != null))
            {
                DiscordDmChannel dm = client.CreateDmChannelAsync().Result;
                var message = new DiscordEmbedBuilder();
                message.Title = e.Title;
                message.Description = e.Description;
                message.Timestamp = e.CreatedDate;
                message.WithAuthor(e.Username);
                dm.SendMessageAsync(message);
                ErrorTaskService.AddErrorTask(e.Title, e.Description, e.CreatedDate, e.Status, e.Username);
            }
        }
    }

    private static Task client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        return Task.CompletedTask;
    }
}
