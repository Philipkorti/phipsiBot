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
    public class MessageFilterService
    {
        public static void AddWords(string word)
        {
            using(var db = new BotContext())
            {
                db.FilterWords.Add(new FilterWords() { Words = word});
                db.SaveChanges();
            }
        }
        public static List<FilterWords> GetWordsList()
        {
            List<FilterWords> words;
            using (var db = new BotContext())
            {
                words = db.FilterWords.ToList();
            }
            return words;
        }
    }
}
