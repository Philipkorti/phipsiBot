using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Services.Services;
using Services.DbServices;
using DSharpPlus.CommandsNext;
using NLog;
using Services.Data;
using Enums.Enums;
using Newtonsoft.Json.Linq;
using Config;
using System.Threading.Channels;
using DbConnection.Entity;
using Services.ConstVariablen;

namespace EventService.Events
{
    public class CheckPatch
    {
        
        public static void CheckPatchNotes(object sender)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            var state = (Tuple<string, CommandContext>)sender;
            string name = state.Item1;
            CommandContext context = state.Item2;
            try
            {
                logger.Info($"Das Service mit dem Namen {name} wird ausgeführt!"); 
                DbConnection.Entity.Services service = ServicesService.GetServiceByName(name);
                Int64 id = Convert.ToInt64(service.ChannelId);
                DiscordChannel channel = context.Client.GetChannelAsync((ulong)id).Result;
                List<ReadNews> readNews = ReadNewsService.GetNewsByServiceId(service.Id);

                foreach(ReadNews read in readNews)
                {
                    switch (read.NewsType)
                    {
                        case NewsType.Steam:
                            {
                                CheckSteam(read.GameLink,channel, read.Name);
                                break;
                            }
                        case NewsType.YouTube:
                            {
                                CheckYouTube(read.GameLink, channel,read.Name);
                                break;
                            }
                    }
                }
                ServicesService.SetServiceStatusByName(name, ServiceStatus.Online);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Es ist ein Fehler passiert!");
                ServicesService.SetServiceStatusByName(name, ServiceStatus.Error);
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace, DateTime.Now, ErrorTaskStatus.NEW.ToString(), "");
            }
           
        }

        private static async void CheckYouTube(string channelId, DiscordChannel channel, string name)
        {
            ConfigReader configReader = new ConfigReader();
            await configReader.ReadConfig();
            using (var httpClient = new HttpClient())
            {
                // Baue die YouTube API Anfrage
                var requestUrl = $"{ConstVariablen.YouTubeAPI}{configReader.YouTubeAPI}&part=snippet&channelId={channelId}&order=date&maxResults=1";
                var response = await httpClient.GetStringAsync(requestUrl);
                var json = JObject.Parse(response);

                var videoId = json["items"]?[0]?["id"]?["videoId"]?.ToString();
                DateTime dateTime = Convert.ToDateTime(json["items"]?[0]?["snippet"]?["publishTime"].ToString());

                ReadNews readNews = ReadNewsService.GetGameNewsDateByName(name);

                if(readNews != null)
                {
                    if(dateTime > readNews.LastUpdate)
                    {
                        if (!string.IsNullOrEmpty(videoId))
                        {
                            ReadNewsService.SetGameNewsDateByName(dateTime, name);
                            var videoTitle = json["items"]?[0]?["snippet"]?["title"]?.ToString();
                            var videoUrl = $"https://www.youtube.com/watch?v={videoId}";

                            // Poste das Video in einen bestimmten Discord-Channel // Ersetze CHANNEL_ID mit der Discord-Channel-ID
                            await channel.SendMessageAsync($"Neues YouTube-Video veröffentlicht: **{videoTitle}**\n{videoUrl}");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        ReadNewsService.SetGameNewsDateByName(dateTime, name);
                        var videoTitle = json["items"]?[0]?["snippet"]?["title"]?.ToString();
                        var videoUrl = $"https://www.youtube.com/watch?v={videoId}";

                        // Poste das Video in einen bestimmten Discord-Channel // Ersetze CHANNEL_ID mit der Discord-Channel-ID
                        await channel.SendMessageAsync($"Neues YouTube-Video veröffentlicht: **{videoTitle}**\n{videoUrl}");
                    }
                }


                
            }
        }

        private static void CheckSteam(string appid, DiscordChannel channel, string name)
        {
            string title = SteamAPIService.GetOverwatchNews(out IEnumerable<string> notes, appid, name);
            if(!string.IsNullOrEmpty(title))
            {
                channel.SendMessageAsync(title);
                foreach (var item in notes)
                {
                    channel.SendMessageAsync(item);
                }
            }
        }
    }
}
