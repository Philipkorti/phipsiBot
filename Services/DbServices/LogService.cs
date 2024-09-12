using DbConnection.Context;
using DbConnection.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DbServices
{
    public class LogService
    {
        public static List<Logging> GetLogs(string date, string loglevel)
        {
            List<Logging> logs = new List<Logging>();
            using (var db = new BotContext())
            {
                var query = db.Loggings.AsQueryable();

                if (!string.IsNullOrEmpty(date))
                {
                    DateTime startDate = Convert.ToDateTime(date);
                    DateTime endDate = startDate.AddDays(1);
                    query = query.Where(log => log.date >= startDate && log.date < endDate);
                }

                if (!string.IsNullOrEmpty(loglevel))
                {
                    query = query.Where(log => string.Equals(log.LogLevel.Trim(),loglevel.Trim()));
                }
               logs = query.ToList();
            }
            return logs;
        }
    }
}
