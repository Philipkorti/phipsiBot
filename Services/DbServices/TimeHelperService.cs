using DbConnection.Context;
using DbConnection.Entity;
using Services.Data;
using Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DbServices
{
    public class TimeHelperService
    {
        public static void SetJoinTime(string username)
        {
            User user = UserServices.GetUerByUsername(username);
            if ((user == null))
            {
                UserServices.AddUser(username);
            }
            
            using (var db = new BotContext())
            {
                try
                {
                    db.Add(new TimeHelper()
                    {
                        User = db.Users.Single(d => d.Username == username),
                        Jointime = DateTime.Now,
                    });

                    db.SaveChanges();
                }catch (Exception ex)
                {
                    ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),username);
                }
                

            }
        }
        public static void ClearJoinTime(string username)
        {
            User user = UserServices.GetUerByUsername(username);
            using(var db = new BotContext())
            {
                try
                {
                    TimeHelper timeHelper = db.TimeHelper.Single(d => d.User == user);
                    if (timeHelper != null)
                    {
                        db.Remove(timeHelper);
                        db.SaveChanges();
                    }
                }catch (Exception ex)
                {
                    ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(),username);
                }
            }
        }

        public static DateTime GetJoinTime(string username)
        {
            DateTime joinTime = DateTime.Now;
            try
            {
                User user = UserServices.GetUerByUsername(username);
                using (var db = new BotContext())
                {
                    joinTime = db.TimeHelper.Single(d => d.User == user).Jointime;
                }
            }
            catch (Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message, ex.StackTrace,DateTime.Now, ErrorTaskStatus.NEW.ToString(),username);
            }
            return joinTime;
        }

        public static List<User> GetTopUsers()
        {
            List<User> users = new List<User>();
            try
            {
                using(var db = new BotContext())
                {
                    users = db.Users.OrderByDescending(d => d.TimeInSecond).Take(5).ToList();
                }
            }
            catch(Exception ex)
            {
                ErrorTaskData errorTaskData = new ErrorTaskData(ex.Message,ex.StackTrace,DateTime.Now,ErrorTaskStatus.NEW.ToString(), null);
            }
            return users;
        }
    }
}
