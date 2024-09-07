using DbConnection.Context;
using DbConnection.Entity;
using Enums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Services.DbServices
{
    public static class UserServices
    {
        public static void AddUser(string username)
        {
            using (var db = new BotContext())
            {
                db.Add(new User
                {
                    Username = username,
                    TimeInSecond = 0
                });
                db.SaveChanges();
            }
        }

        public static User GetUerByUsername(string username)
        {
            User user;
            using (var db = new BotContext())
            {
                user = db.Users.SingleOrDefault(user => user.Username == username);
            }
            return user;
        }

        public static void SetUserTime(string username, Int64 timeInSeconds)
        {
            using(var db = new BotContext())
            {
                db.Users.Single(d=>d.Username == username).TimeInSecond += timeInSeconds;
                db.SaveChanges();
            }
        }

        public static Int64 GetUserTimeByUsername(string username)
        {
            Int64 userTime;
            using(var db = new BotContext())
            {
                userTime = db.Users.Single(user=>user.Username == username).TimeInSecond;
            }

            return userTime;
        }

        public static void SetGroupByUsername(string username, Groups groups)
        {
            using (var db = new BotContext())
            {
                User user = db.Users.Single(user => user.Username == username);
                user.Groups = groups;
                db.Users.Update(user);
                db.SaveChanges();
            }
        }

        public static User GetUserById(int id)
        {
            User user;
            using (var db = new BotContext())
            {
                user = db.Users.SingleOrDefault(user =>user.Id == id);
            }
            return user;
        }
    }
}
