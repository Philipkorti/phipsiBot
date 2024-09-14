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
                if (!Authentication.IsUserAuthorized(this, nameof(GetServices), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                List<DbConnection.Entity.Services> services = ServicesService.GetServices();
                string msg;
                foreach( DbConnection.Entity.Services service in services)
                {
                    msg = $"Service: {service.Title} Status: {service.Status} ChannelId: {service.ChannelId}";
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
                if (!Authentication.IsUserAuthorized(this, nameof(SetChannelId), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                ServicesService.SetChannelId(name,ctx.Channel.Id);
                string msg = $"Die ChannelId von {name} wurde auf {ctx.Channel.Id} gesetzt!";
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
                if (!Authentication.IsUserAuthorized(this, nameof(StartService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(name);
                if(services == null)
                {
                    msg = $"Das Service mit dem Namen {name} gibt es nicht!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }

                if(services.Status == ServiceStatus.Online)
                {
                    msg = $"Das Service {name} ist schon gestartet!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                ServicesService.SetServiceStatusByName(name, ServiceStatus.Starting);
                msg = $"Das Service {name} wird gestartet...";
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
                if (!Authentication.IsUserAuthorized(this, nameof(CreateService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                string msg;
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(name);
                if (services != null)
                {
                    msg = $"Das Service mit dem Namen {name} gibt es schon!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                IntervallType intervalType = (IntervallType)intervalTyp;
                ServicesService.CreateServices(name, interval, intervalType);
                msg = $"Das Service mit dem Namen {name} wurde erstellt! Interval {interval} IntervalType {intervalType.ToString()}";
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
                if (!Authentication.IsUserAuthorized(this, nameof(SetGameNews), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(serviceName);
                string msg;
                if(services == null)
                {
                    msg = $"Das Service mit dem Namen {serviceName} gibt es nicht!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                NewsType news = new NewsType();
                news = (NewsType)newsType;
                ReadNewsService.AddGameNews(services.Id, gameInfo, news);
                msg = $"Das spiel {gameInfo} wurde zu dem Service {serviceName} hinzugefügt!";
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
                if (!Authentication.IsUserAuthorized(this, nameof(StopService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }

                timer.Remove(name);
                ServicesService.SetServiceStatusByName(name, ServiceStatus.Offline);
                string msg = $"Das Service {name} wurde gestoppt!";
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
                if (!Authentication.IsUserAuthorized(this, nameof(StopService), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                DbConnection.Entity.Services services = ServicesService.GetServiceByName(name);
                string msg;
                if(services == null)
                {
                    msg = $"Das Service mit dem Namen {name} gibt es nicht!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }

                if (services.Status == ServiceStatus.Online)
                {
                    msg = $"Das Service {name} ist gestartet sie müssen es vorher stoppen!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                ServicesService.RemoveService(name);
                msg = $"Das Service {name} wurde gelöscht!";
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
                if (!Authentication.IsUserAuthorized(this, nameof(RemoveReadNews), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                ReadNews readNews = ReadNewsService.GetGameNewsDateByName(name);
                string msg;
                if(readNews == null)
                {
                    msg = $"Das Service mit dem Namen {name} gibt es nicht!";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                ReadNewsService.RemoveReadNews(name);
                msg = $"Das ReadNews {name} wurde gelöscht!";
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
                if (!Authentication.IsUserAuthorized(this, nameof(GetAllReadNews), ctx.User.Username))
                {
                    logger.Warn($"Der User {ctx.User.Username} hat einen Befehl benutzt wo für er nicht berechtigt ist!");
                    await ctx.Channel.SendMessageAsync("Sie dürfen diesen Befehl nicht ausführen!");
                    return;
                }
                List<ReadNews> readNews = ReadNewsService.ReadNews();
                string msg;
                if(readNews == null)
                {
                    msg = "Es gibt keine ReadNews";
                    logger.Warn(msg);
                    await ctx.Channel.SendMessageAsync(msg);
                    return;
                }
                foreach(ReadNews read in readNews)
                {
                    await ctx.Channel.SendMessageAsync($"{read.Name}, Game Connection {read.GameLink}, Last Update: {read.LastUpdate}");
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
