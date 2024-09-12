using DbConnection.Context;
using DbConnection.Entity;
using Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.DbServices
{
    public static class ErrorTaskService
    {
        public static void AddErrorTask(string title, string description, DateTime createdDate, string status, string username)
        {
            using (var db = new BotContext())
            {
                db.Add(new ErrorTask()
                {
                    Title = title,
                    Description = description,
                    CreatedDate = createdDate,
                    Status = status,
                    UserId = UserServices.GetUerByUsername(username).Id,
                });
                db.SaveChanges();
            }
        }
    }
}
