using DbConnection.Context;
using DbConnection.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
