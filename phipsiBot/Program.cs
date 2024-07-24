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
public class Program
{
    private static DiscordClient client { get; set; }
    private static CommandsNextExtension commands { get; set; }

    static async Task Main(string[] args)
    {
        var configReader = new ConfigReader();
        await configReader.ReadConfig();
        

        var discordConfig = new DiscordConfiguration()
        {
            Intents = DiscordIntents.All,
            Token = configReader.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };

        client = new DiscordClient(discordConfig);
        client.Ready += client_Ready;
        client.VoiceStateUpdated += DiscordTimeCommand.OnVoiceStateUpdate;
        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new string[] { configReader.Pefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
        };
        commands = client.UseCommandsNext(commandsConfig);
        commands.RegisterCommands<TestCommand>();
        commands.RegisterCommands<AddUser>();
        commands.RegisterCommands<DiscordTimeCommand>();
        await client.ConnectAsync();
        await Task.Delay(-1);
    }

    private static Task client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        return Task.CompletedTask;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigReader configReader = new ConfigReader();
        services.AddDbContext<BotContext>(options => options.UseMySQL(configReader.dbConnection));
    }
}
