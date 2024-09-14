using Config;
using DbConnection.Context;
using DbConnection.Entity;
using Enums.Enums;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata;
using MySqlX.XDevAPI;
using Newtonsoft.Json.Linq;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Services.DbServices
{
    public class ReadNewsService
    {
        public static List<ReadNews> GetNewsByServiceId(int serviceId)
        {
            List<ReadNews> readNews = new List<ReadNews>();
            using (var db = new BotContext())
            {
                readNews = db.ReadNews.Where(s=> s.ServiceId == serviceId).ToList();
            }

            return readNews;
        }
        public static void AddGameNews(int serviceId, string gameinfo, NewsType news)
        {
            string name = news == NewsType.Steam ? GetGameNameSteam(gameinfo) : GetYuTubeName(gameinfo).Result;
            using(var db = new BotContext())
            {
                db.ReadNews.Add(new ReadNews()
                {
                    Name = name,
                    GameLink = gameinfo,
                    ServiceId = serviceId,
                    NewsType = news,
                });
                db.SaveChanges();
            }
        }

        public static ReadNews GetGameNewsDateByName(string name)
        {
            ReadNews readNews;
            using(var db = new BotContext())
            {
                readNews = db.ReadNews.FirstOrDefault(s => s.Name == name);
            }
            return readNews;
        }

        public static void SetGameNewsDateByName(DateTime date, string name)
        {
            using(var db = new BotContext())
            {
                db.ReadNews.SingleOrDefault(s=> s.Name == name).LastUpdate = date;
                db.SaveChanges();
            }
        }

        private static string GetGameNameSteam(string appId)
        {
            string url = $"http://store.steampowered.com/api/appdetails?appids={appId}";
            using (HttpClient client = new HttpClient())
            {
                // HTTP GET - Anfrage an die API senden
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                // Antwort als String erhalten
                string responseBody = response.Content.ReadAsStringAsync().Result;

                // JSON-Daten parsen
                JObject data = JObject.Parse(responseBody);

                // Prüfen, ob der Abruf erfolgreich war
                if (data[appId.ToString()]["success"].Value<bool>())
                {
                    // Spielname extrahieren
                    string gameName = data[appId.ToString()]["data"]["name"].ToString();
                    return gameName;
                }
                else
                {
                    return null; // Spiel nicht gefunden
                }
            }
        }

        private static async Task<string>  GetYuTubeName(string appId)
        {
            ConfigReader configReader = new ConfigReader();
            await configReader.ReadConfig();
            string channelName;
            string url = $"https://www.googleapis.com/youtube/v3/channels?key={configReader.YouTubeAPI}&part=snippet&id={appId}";
            using (HttpClient client = new HttpClient())
            {
                // HTTP GET-Anfrage an die API senden
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Antwort als String erhalten
                string responseBody = await response.Content.ReadAsStringAsync();

                // JSON-Daten parsen
                JObject data = JObject.Parse(responseBody);

                // Kanalname ausgeben
                channelName = data["items"][0]["snippet"]["title"].ToString();
            }
            return channelName;
        }

        public static void RemoveReadNews(string name)
        {
            using (var db = new BotContext())
            {
                var game = db.ReadNews.SingleOrDefault(s => s.Name == name);
                db.ReadNews.Remove(game);
                db.SaveChanges();
            }
        }

        public static List<ReadNews> ReadNews()
        {
            List<ReadNews> readNews = new List<ReadNews>();
            using (var db = new BotContext())
            {
                readNews = db.ReadNews.ToList();
            }
            return readNews;
        }
    }
}
