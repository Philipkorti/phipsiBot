using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnection.Context;
using DbConnection.Entity;
using Enums.Enums;

namespace Services.DbServices
{
    public class ServicesService
    {
        public static List<DbConnection.Entity.Services> GetServices()
        {
            List<DbConnection.Entity.Services> services;
            using(var db = new BotContext())
            {
                services = db.Services.ToList();
            }
            return services;
        }

        public static void SetChannelId(string name, ulong channelId)
        {
            using(var db = new BotContext())
            {
                DbConnection.Entity.Services services = db.Services.Single(s => s.Title == name);
                services.ChannelId = channelId.ToString();
                db.SaveChanges();
            }
        }

        public static DbConnection.Entity.Services GetServiceByName(string name)
        {
            DbConnection.Entity.Services service;
            using(var db = new BotContext())
            {
                service = db.Services.SingleOrDefault(s => s.Title == name);
            }

            return service;
        }

        public static void SetServiceStatusByName(string name, ServiceStatus status)
        {
            using( var db = new BotContext())
            {
                db.Services.Single(s=>s.Title == name).Status = status;
                db.SaveChanges();
            }
        }

        public static void CreateServices(string name, int interval, IntervallType intervalType)
        {
            using(var db =new BotContext())
            {
                db.Services.Add(new DbConnection.Entity.Services()
                {
                    Title = name,
                    ChannelId = "",
                    Status = ServiceStatus.Offline,
                    Intervall = interval,
                    IntervallType = intervalType
                });
                db.SaveChanges();
            }
        }

        public static void SetAllOffline()
        {
            List<DbConnection.Entity.Services> services;
            using(var db = new BotContext())
            {
                services = db.Services.ToList();
                foreach(var service in services)
                {
                    service.Status = ServiceStatus.Offline;
                }
                db.SaveChanges();
            }
        }

        public static void RemoveService(string name)
        {
            using( var db = new BotContext())
            {
                var service = db.Services.SingleOrDefault(s=>s.Title == name);
                db.Services.Remove(service);
                db.SaveChanges();
            }
        }
    }
}
