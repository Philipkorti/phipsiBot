using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Services;
using Enums.Enums;
using NLog;
using Services.Data;
using Commands.Authorized;
using Services.DbServices;
using System.Threading;
using Services.Events;
using EventService.Events;
using System.Net.Http.Headers;
using DbConnection.Entity;
using Language;

namespace Commands
{
    public class ServiceCommands : BaseCommandModule
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        Dictionary<string, Timer> timer = new Dictionary<string, Timer>();

        [Command("Services")]
        [GroupsService(Groups.Manager)]
        public async Task GetServices(CommandContext ctx)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl Services benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(GetServices), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                List<DbConnection.Entity.Services> services = ServicesService.GetServices();
                string msg;
                foreach( DbConnection.Entity.Services service in services)
                {
                    msg = botLocalization.GetLocalizedString("services", service.Title,service.Status,service.ChannelId);
                    logger.Info(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("SetChannelId")]
        [GroupsService(Groups.Manager)]
        public async Task SetChannelId(CommandContext ctx, string name)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl SetChannelId benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(SetChannelId), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                ServicesService.SetChannelId(name,ctx.Channel.Id);
                string msg;
                msg = botLocalization.GetLocalizedString("setChannelId", name,ctx.Channel.Id);
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("start")]
        [GroupsService(Groups.Admin)]
        public async Task StartService(CommandContext ctx, string name)
        {
            try
            {
                string msg;
                logger.Info($"Der User {ctx.User.Username} hat den Befehl start benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(StartService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(name);
                if(services == null)
                { 
                     msg = botLocalization.GetLocalizedString("serviceNotExists", name);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }

                if(services.Status == ServiceStatus.Online)
                {
                    msg = botLocalization.GetLocalizedString("serviceAlreadyOnline", name);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                ServicesService.SetServiceStatusByName(name, ServiceStatus.Starting);
                msg = botLocalization.GetLocalizedString("starting", name);
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);

                var state = Tuple.Create(name, ctx);
                TimeSpan timeSpan = TimeSpan.FromSeconds(0);
                switch (services.IntervallType)
                {
                    case IntervallType.Second:
                        {
                            timeSpan = TimeSpan.FromSeconds(services.Intervall);
                            break;
                        }
                    case IntervallType.Minute:
                        {
                            timeSpan = TimeSpan.FromMinutes(services.Intervall);
                            break;
                        }
                    case IntervallType.Hours:
                        {
                            timeSpan = TimeSpan.FromHours(services.Intervall);
                            break;
                        }
                }
                timer.Add(name, new Timer(CheckPatch.CheckPatchNotes,state,0,Convert.ToInt32(Math.Round(timeSpan.TotalMilliseconds,0))));
                msg = $"Das Service {services.Title} ist gestartet.";
                msg = botLocalization.GetLocalizedString("online",services.Title);
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
        }

        [Command("create")]
        [GroupsService(Groups.Admin)]
        public async Task CreateService(CommandContext ctx,string name, int interval, int intervalTyp)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl create benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(CreateService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                string msg;
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(name);
                if (services != null)
                {
                    msg = botLocalization.GetLocalizedString("serviceAlreadyExists",name);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                IntervallType intervalType = (IntervallType)intervalTyp;
                ServicesService.CreateServices(name, interval, intervalType);
                msg = botLocalization.GetLocalizedString("serviceCreate", name,interval,intervalTyp.ToString());
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
        [Command("addgame")]
        [GroupsService(Groups.Manager)]
        public async Task SetGameNews(CommandContext ctx, string serviceName,string gameInfo, int newsType)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl addgame benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(SetGameNews), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(serviceName);
                string msg;
                if(services == null)
                {
                    msg = botLocalization.GetLocalizedString("serviceNotExists",serviceName);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                NewsType news = new NewsType();
                news = (NewsType)newsType;
                ReadNewsService.AddGameNews(services.Id, gameInfo, news);
                msg = botLocalization.GetLocalizedString("addGame", gameInfo, serviceName);
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("stop")]
        [GroupsService(Groups.Admin)]
        public async Task StopService(CommandContext ctx, string name)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl stop benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(StopService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }

                timer.Remove(name);
                ServicesService.SetServiceStatusByName(name, ServiceStatus.Offline);
                string msg;
                msg = botLocalization.GetLocalizedString("serviceStop",name);
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),ctx.User.Username);
            }
        }

        [Command("removeService")]
        [GroupsService(Groups.Admin)]
        public async Task RemoveService(CommandContext ctx, string name)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl removeService benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(StopService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(name);
                string msg;
                if(services == null)
                {
                    msg = botLocalization.GetLocalizedString("serviceNotExists", name);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }

                if (services.Status == ServiceStatus.Online)
                {
                    msg = botLocalization.GetLocalizedString("errorservicerunning", name);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                ServicesService.RemoveService(name);
                msg = botLocalization.GetLocalizedString("deleteService", name);
                logger.Info(name);
                await ctx.Channel.SendMessageAsync(msg);
            }catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("removeReadNews")]
        [GroupsService(Groups.Manager)]
        public async Task RemoveReadNews(CommandContext ctx, string name)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl removeReadNews benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(RemoveReadNews), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                ReadNews readNews = ReadNewsService.GetGameNewsDateByName(name);
                string msg;
                if(readNews == null)
                {
                    msg = botLocalization.GetLocalizedString("serviceNotExists", name);
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                ReadNewsService.RemoveReadNews(name);
                msg = botLocalization.GetLocalizedString("deleteGame", name);
                logger.Info(msg);
                await ctx.Channel.SendMessageAsync(msg);
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }

        [Command("readnews")]
        [GroupsService(Groups.Manager)]
        public async Task GetAllReadNews(CommandContext ctx)
        {
            try
            {
                logger.Info($"Der User {ctx.User.Username} hat den Befehl readnews benutzt!");
                BotLocalization botLocalization = new BotLocalization(UserServices.GetLanguageCodeByUsername(ctx.User.Username));
                if (!Authentication.IsUserAuthorized(this, nameof(GetAllReadNews), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync(botLocalization.GetLocalizedString("errorCommand"));
                    return;
                }
                List<ReadNews> readNews = ReadNewsService.ReadNews();
                string msg;
                if(readNews == null)
                {
                    msg = botLocalization.GetLocalizedString("errorviewReadNews");
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                foreach(ReadNews read in readNews)
                {
                    msg = botLocalization.GetLocalizedString("viewReadNews",read.Name,read.GameLink,read.LastUpdate);
                    await ctx.Channel.SendMessageAsync(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Es ist ein Fehler aufgetreten bei dem User {ctx.User.Username}");
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), ctx.User.Username);
            }
        }
    }
}
