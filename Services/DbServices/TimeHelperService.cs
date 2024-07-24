﻿using DbConnection.Context;
using DbConnection.Entity;
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

                }
                
               
            }
        }

        public static DateTime GetJoinTime(string username)
        {
            DateTime joinTime;
            User user = UserServices.GetUerByUsername(username);
            using(var db = new BotContext())
            {
                joinTime = db.TimeHelper.Single(d => d.User == user).Jointime;
            }
            return joinTime;
        }
    }
}
