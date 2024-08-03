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
    public static class ErrorTaskService
    {
        public static void AddErrorTask(string title, string description, string username)
        {
            ErrorTaskData task = new ErrorTaskData(title,description,DateTime.Now, ErrorTaskStatus.NEW.ToString(),username);
            using (var db = new BotContext())
            {
                db.Add(new ErrorTask()
                {
                    Title = task.Title,
                    Description = task.Description,
                    CreatedDate = task.CreatedDate,
                    Status = task.Status,
                    UserId = UserServices.GetUerByUsername(task.Username).Id,
                });
                db.SaveChanges();
            }
        }
    }
}
